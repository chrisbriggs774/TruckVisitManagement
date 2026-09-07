ADR-003: Event-Driven Projection Strategy 

Status 

Accepted 

 

Context 

The Truck Visit Management platform has adopted a CQRS architecture (ADR-001). 

Under this architecture: 

    DynamoDB is the transactional system of record. 

    OpenSearch is the query-optimised read model. 

    Writes and reads are independently optimised. 

    Search capabilities are expected to evolve over time. 

    Read traffic is expected to be lower than write traffic. 

The platform must support: 

    Near real-time operational visibility 

    Flexible search capabilities 

    Future reporting requirements 

    Scalable read and write workloads 

    99.95% availability 

Since read and write models are stored in different persistence technologies, a mechanism is required to keep the OpenSearch read model synchronised with changes made in DynamoDB. 

 

Decision 

The solution will use an event-driven projection architecture to build and maintain the OpenSearch read model. 

Changes committed to DynamoDB will be published through DynamoDB Streams and processed asynchronously by a projection service. 

1                     +----------------+ 

2                     | Truck Visit API| 

3                     +--------+-------+ 

4                              | 

5                              v 

6                     +----------------+ 

7                     |   DynamoDB     | 

8                     | System of      | 

9                     | Record         | 

10                     +--------+-------+ 

11                              | 

12                              v 

13                     +----------------+ 

14                     | DynamoDB Stream | 

15                     +--------+-------+ 

16                              | 

17                              v 

18                     +----------------+ 

19                     | Projection     | 

20                     | Service        | 

21                     +--------+-------+ 

22                              | 

23                              v 

24                     +----------------+ 

25                     | OpenSearch     | 

26                     | Read Model     | 

27                     +----------------+ 

OpenSearch will never be updated directly by API requests. 

All read model updates must originate from events emitted from the transactional datastore. 

 

Rationale 

Decoupling Reads from Writes 

The transactional model and query model serve different purposes. 

DynamoDB is optimised for: 

    Fast writes 

    Aggregate persistence 

    Operational simplicity 

OpenSearch is optimised for: 

    Multi-field filtering 

    Search 

    Aggregations 

    Analytics 

    Operational dashboards 

Separating these concerns allows each technology to be optimised independently. 

 

Scalability 

As the number of terminals grows, write throughput and search workload are likely to grow at different rates. 

Event-driven projections allow: 

    Independent scaling of projection processing 

    Independent scaling of search infrastructure 

    Independent tuning of read and write performance 

Without introducing coupling between systems. 

 

Future Query Flexibility 

Search requirements frequently evolve. 

Examples include: 

    Additional filter combinations 

    New reporting attributes 

    Operational dashboards 

    Regulatory reporting support 

By projecting data into OpenSearch, new indexes and document structures can be introduced without impacting the transactional data model. 

 

Resilience 

The write path remains available even if OpenSearch is temporarily unavailable. 

This is important because: 

1     Creating Visits 

2     Updating Statuses 

3     Recording Audit Data 

are business-critical operations. 

Search is important but should not prevent business transactions from being recorded. 

 

Projection Architecture 

Event Source 

DynamoDB Streams will act as the source of changes. 

Events generated include: 

1     Visit Created 

2     Visit Updated 

3     Visit Status Changed 

4     Visit Expired 

 

Projection Service 

The Projection Service is responsible for: 

    Consuming stream events 

    Transforming domain data 

    Building search documents 

    Updating OpenSearch indexes 

    Managing projection failures 

The Projection Service contains no business logic. 

It exists solely to create and maintain read models. 

 

Search Document Example 

A visit projection may contain: 

1     VisitId 

2     TerminalId 

3     CurrentStatus 

4     TruckNumber 

5     TrailerNumber 

6     DriverName 

7     MovementDates 

8     CreatedBy 

9     CreatedDate 

10     LastUpdatedDate 

This structure is optimised for querying rather than transactional consistency. 

 

Consistency Model 

The platform adopts an Eventual Consistency model. 

Business Acceptance 

Immediately after a successful write: 

1     Get By Id 

must return the latest transactional state. 

However: 

1     Search 

may take a short period to reflect changes. 

 

Target Projection SLA 

The platform should target: 

1     95% of projections < 2 seconds 

2     99% of projections < 5 seconds 

from successful transaction commit to search availability. 

 

User Experience Implications 

Operational users may observe: 

1     Visit created 

2     Search immediately performed 

3     Visit not yet visible 

This behaviour is expected and should be documented. 

 

Failure Handling 

Scenario 

Projection processing fails due to: 

    OpenSearch outage 

    Network interruption 

    Temporary infrastructure failure 

 

Behaviour 

Transactional writes remain successful. 

The Projection Service retries processing until successful. 

1     Write Success 

2           | 

3           v 

4     Projection Failure 

5           | 

6           v 

7     Retry Queue 

8           | 

9           v 

10     Successful Projection 

 

Dead Letter Queue 

Failed projections exceeding retry limits will be moved to a Dead Letter Queue (DLQ). 

This allows: 

    Operational investigation 

    Replay capability 

    Controlled recovery 

 

Idempotency 

Projection processing must be idempotent. 

Processing the same event multiple times must produce the same result. 

Example: 

1     Visit Status Changed 

processed twice must not create duplicate documents. 

This protects against: 

    Stream retries 

    Consumer restarts 

    Network failures 

 

Retention Alignment 

ADR-001 establishes a seven-year retention policy. 

The projection strategy must honour the same lifecycle. 

Rule 

When records expire from DynamoDB: 

1     TTL Reached 

2           | 

3           v 

4     Expiration Event 

5           | 

6           v 

7     Remove Projection 

8           | 

9           v 

10     OpenSearch Cleanup 

The search index must not retain records beyond the approved retention period. 

 

Alternatives Considered 

Dual Writes 

1     API 

2      | 

3      +--> DynamoDB 

4      | 

5      +--> OpenSearch 

Advantages 

    Immediate search consistency 

Disadvantages 

    Increased application complexity 

    Partial failure scenarios 

    Inconsistent data risk 

    Tight coupling 

Decision 

Rejected. 

 

Synchronous Projection 

1     Write 

2       | 

3       v 

4     Update Search 

5       | 

6       v 

7     Return Success 

Advantages 

    Stronger consistency 

Disadvantages 

    Slower write path 

    Search dependency on critical transactions 

    Reduced resilience 

Decision 

Rejected. 

 

Scheduled Batch Synchronisation 

1     DynamoDB 

2        | 

3        v 

4     Hourly Sync Job 

5        | 

6        v 

7     OpenSearch 

Advantages 

    Simpler implementation 

Disadvantages 

    Poor operational visibility 

    Significant data latency 

    Fails near real-time requirements 

Decision 

Rejected. 

 

Operational Requirements 

Monitoring 

The Projection Service must expose metrics for: 

1     Projection Throughput 

2     Projection Latency 

3     Projection Failures 

4     Retry Count 

5     DLQ Count 

6     OpenSearch Indexing Latency 

 

Alerting 

Operational alerts should be raised when: 

1     Projection Backlog Exceeds Threshold 

2     DLQ Contains Messages 

3     Projection Latency SLA Breached 

4     OpenSearch Indexing Fails 

 

Health Checks 

Health endpoints should validate: 

1     DynamoDB Stream Connectivity 

2     OpenSearch Connectivity 

3     Projection Consumer Status 

 

Consequences 

Positive 

    Highly scalable architecture 

    Decoupled read and write models 

    Resilient transaction processing 

    Flexible search capabilities 

    Independent optimisation of each datastore 

    Supports future reporting requirements 

 

Negative 

    Eventual consistency 

    Additional infrastructure components 

    Increased operational monitoring requirements 

    Replay and recovery mechanisms required 

 

Risks 

Risk: Projection Backlog Growth 

Large event volumes could delay search visibility. 

Mitigation 

    Horizontal scaling of projection processors 

    Backlog monitoring 

    Alerting on latency thresholds 

 

Risk: Search Model Drift 

Projection failures could cause OpenSearch to diverge from DynamoDB. 

Mitigation 

    Replay capability 

    Periodic reconciliation jobs 

    DLQ monitoring 

 

Risk: Event Schema Evolution 

Future model changes may break projections. 

Mitigation 

    Versioned event contracts 

    Backward-compatible schema changes 

    Controlled reindexing processes 


Architecture Principle Established 

All OpenSearch read models will be maintained asynchronously through an event-driven projection process sourced from DynamoDB Streams. DynamoDB remains the authoritative source of truth, while OpenSearch acts as a near real-time searchable projection optimised for operational queries. 