ADR-004: Immutable Audit History 

Status 

Accepted 

 

Context 

The Truck Visit Management platform must satisfy the following business and operational requirements: 

    Regulatory audits occur regularly 

    Full audit history of status changes is required 

    Audit records must be retained for 7 years 

    Visit status can be updated throughout its lifecycle 

    The platform must provide traceability of operational activity 

    Multiple actors may interact with a Visit over time 

A Visit progresses through a defined lifecycle: 

1     Pre-Registered 

2         ↓ 

3     At Gate 

4         ↓ 

5     On Site 

6         ↓ 

7     Completed 

Whilst the current state of a Visit may change, the business requires a permanent record of how that state was reached. 

Without an immutable audit capability, the platform would be unable to reliably answer questions such as: 

    When did the truck arrive? 

    Who changed the status? 

    What was the previous status? 

    Has data been altered after the fact? 

    Can the history be trusted during an audit? 

A dedicated audit strategy is therefore required. 

 

Decision 

All Visit status transitions shall be recorded as immutable audit records. 

Audit records: 

    Are append-only 

    Cannot be modified 

    Cannot be deleted before retention expiry 

    Must be retained for seven years 

    Must be independently queryable 

    Must provide a complete lifecycle history for a Visit 

Every successful status transition will generate a new audit record. 

Existing audit records are never updated. 

 

Audit Model 

Current State 

The Visit Aggregate stores the current state of the Visit. 

Example: 

1     Visit ABC123 

2      

3     Current Status = On Site 

 

Historical State 

Historical transitions are stored as immutable audit records. 

Example: 

1     1. Pre-Registered 

2     2. At Gate 

3     3. On Site 

The current status can always be derived from the last successful transition, but operational APIs are optimised by storing the current status separately. 

 

Audit Record Structure 

Each audit record should contain: 

1     AuditId 

2     VisitId 

3     TerminalId 

4     EventType 

5     PreviousStatus 

6     NewStatus 

7     Timestamp 

8     ActorId 

9     ActorType 

10     CorrelationId 

 

Actor Types 

Possible actor types include: 

1     Driver 

2     Gate Operator 

3     System 

4     Administrator 

5     Integration 

This provides traceability regardless of who initiated the action. 

 

Example Audit Trail 

Visit Creation 

1     Timestamp: 

2     2026-01-01T09:00:00Z 

3      

4     Event: 

5     VisitCreated 

6      

7     Status: 

8     Pre-Registered 

 

Arrival At Gate 

1     Timestamp: 

2     2026-01-01T09:15:00Z 

3      

4     Event: 

5     VisitStatusChanged 

6      

7     Previous: 

8     Pre-Registered 

9      

10     New: 

11     At Gate 

 

Enter Site 

1     Timestamp: 

2     2026-01-01T09:20:00Z 

3      

4     Event: 

5     VisitStatusChanged 

6      

7     Previous: 

8     At Gate 

9      

10     New: 

11     On Site 

 

Visit Completion 

1     Timestamp: 

2     2026-01-01T11:30:00Z 

3      

4     Event: 

5     VisitStatusChanged 

6      

7     Previous: 

8     On Site 

9      

10     New: 

11     Completed 

 

Append-Only Principle 

Audit records are append-only. 

Permitted operations: 

1     INSERT 

Not permitted: 

1     UPDATE 

2     DELETE 

before retention expiry. 

 

Example 

Allowed: 

1     Record 1 

2     Record 2 

3     Record 3 

4     Record 4 

Not allowed: 

1     Modify Record 2 

2     Delete Record 3 

3     Overwrite Record 1 

If an error occurs, a new audit record must be added to represent the correction. 

Historical facts must never be rewritten. 

 

Rationale 

Regulatory Compliance 

The business requirement explicitly states: 

1     Full audit history of status changes 

Immutable records provide a verifiable audit trail. 

 

Trustworthiness 

An audit trail can only be trusted if historical records cannot be altered. 

Immutability ensures: 

    Accountability 

    Non-repudiation 

    Traceability 

 

Operational Investigations 

Audit records support investigation of: 

    Driver complaints 

    Gate incidents 

    Operational delays 

    Status discrepancies 

    Security reviews 

 

Architectural Simplicity 

Append-only audit models are simpler to reason about than mutable audit records. 

Every event becomes a historical fact. 

 

DynamoDB Storage Strategy 

Audit records will be stored separately from the mutable Visit Aggregate. 

Recommended structure: 

1     Visits Table 

2      

3     PK = VisitId 

4      

5     CurrentStatus 

6     Truck 

7     Trailer 

8     Driver 

9     ... 

1     VisitAudit Table 

2      

3     PK = VisitId 

4     SK = Timestamp 

This enables efficient retrieval of a Visit's audit history whilst maintaining clear separation between: 

1     Current State 

and 

1     Historical Facts 

 

Retention Strategy 

Audit data must be retained for: 

1     7 Years 

in accordance with operational requirements. 

 

TTL Policy 

Each audit record will contain: 

1     ExpiresAt 

computed as: 

1     CreatedAt + 7 Years 

DynamoDB TTL will be used to automatically expire records once retention obligations have been satisfied. 

 

Important Note 

No audit record may be removed before: 

1     Retention Period Complete 

unless a legal or regulatory process explicitly requires otherwise. 

 

Audit Events 

The following events must create audit entries. 

Visit Created 

1     VisitCreated 

 

Visit Status Changed 

1     VisitStatusChanged 

 

Manual Intervention 

1     ManualReviewPerformed 

 

Data Correction 

1     VisitCorrected 

 

Security-Relevant Actions 

Examples: 

1     UnauthorizedAccessAttempt 

2     AccessGranted 

3     AccessRevoked 

where applicable. 

 

Correlation Requirements 

Every audit event must include a correlation identifier. 

Purpose: 

    Cross-service tracing 

    Operational debugging 

    Distributed request analysis 

Example: 

1     CorrelationId 

2      

3     8ee3d95b-faa2-4708-8a87-babb80136ef3 

All audit events generated during a single workflow should share the same correlation identifier. 

 

Search Considerations 

Audit records are not intended to support operational search workloads. 

Operational visit searches should continue to be served from: 

1     OpenSearch 

Audit records are optimised for: 

1     Traceability 

2     Compliance 

3     Investigation 

4     Historical Review 

 

Alternatives Considered 

Option 1: Status History Embedded Within Visit Only 

Advantages 

    Simpler data model 

Disadvantages 

    Difficult retention management 

    Harder compliance reporting 

    Limits future audit capabilities 

Decision 

Rejected. 

 

Option 2: Mutable Audit Records 

Advantages 

None. 

Disadvantages 

    Breaks auditability 

    Allows historical manipulation 

    Fails compliance objectives 

Decision 

Rejected. 

 

Option 3: Database Change Logs Only 

Advantages 

    Minimal application logic 

Disadvantages 

    Database-specific 

    Difficult to query 

    Difficult to understand from a business perspective 

    Poor portability 

Decision 

Rejected. 

 

Risks 

Risk: Audit Growth 

Seven years of data may create large audit datasets. 

Mitigation 

    DynamoDB TTL 

    Efficient partitioning strategy 

    Archival review if requirements change 

 

Risk: Missing Audit Events 

Development changes could accidentally bypass audit creation. 

Mitigation 

    Audit generation embedded within domain workflows 

    Automated tests verifying audit creation 

    Monitoring of audit event volumes 

 

Risk: Audit and Business State Divergence 

Business updates succeed but audit creation fails. 

Mitigation 

Audit record creation must be treated as part of the successful transaction workflow. 

A state change is not complete until the corresponding audit record has been created. 

 

Consequences 

Positive 

    Meets regulatory requirements 

    Complete lifecycle traceability 

    Supports investigations 

    Provides trusted historical records 

    Supports future reporting and analytics 

    Clear separation between current state and audit history 

 

Negative 

    Increased storage consumption 

    Additional write operations 

    Additional monitoring requirements 

 

Architecture Principle Established 

All Visit status transitions and auditable business actions shall be recorded as immutable, append-only audit records. Historical events are considered permanent business facts and must not be modified or deleted during the mandated seven-year retention period. 