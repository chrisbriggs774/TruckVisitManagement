ADR-002: Host APIs on Kubernetes 

Status 

Accepted 

 

Context 

The Truck Visit Management platform must support: 

    99.95% availability 

    Peak throughput of up to 300 requests per second 

    Horizontal scalability 

    Secure and repeatable deployment processes 

    Enterprise observability standards 

    Future growth across multiple terminals 

DFDS has an established strategic platform for hosting APIs and backend services using Kubernetes. 

Alternative hosting models considered include: 

    AWS Lambda with API Gateway 

    Amazon ECS/Fargate 

    Virtual Machines (EC2) 

    Traditional PaaS offerings 

The Truck Visit Management service is expected to operate as a long-lived API workload that receives a consistent volume of traffic throughout the day, rather than an infrequently invoked event-driven workload. 

 

Decision 

Deploy the Truck Visit Management API to the existing DFDS Kubernetes platform. 

The service will be deployed as a containerized application running within Kubernetes and exposed through the organization's standard ingress and networking architecture. 

1     Clients 

2        | 

3        v 

4     Ingress Controller 

5        | 

6        v 

7     Truck Visit API 

8        | 

9        +--> DynamoDB 

10        | 

11        +--> OpenSearch 

Kubernetes will be the standard runtime environment for: 

    API Hosting 

    Service Scaling 

    Deployment Management 

    Health Monitoring 

    Observability Integration 

 

Rationale 

Alignment with DFDS Standards 

DFDS already operates Kubernetes as the standard platform for hosting APIs. 

Adopting the organisational standard provides: 

    Existing operational knowledge 

    Established support processes 

    Existing monitoring capabilities 

    Reusable deployment pipelines 

    Reduced operational risk 

Architecture should favour platform consistency unless a compelling business reason exists to diverge. 

 

Operational Maturity 

The Kubernetes platform already provides: 

    Container orchestration 

    Automated deployment pipelines 

    Rolling updates 

    Health checks 

    Autoscaling 

    Centralised logging 

    Metrics collection 

    Secrets management 

The Truck Visit Management API can consume these capabilities without additional engineering effort. 

 

Predictable Workload Characteristics 

Truck Visit Management exhibits relatively predictable traffic patterns. 

Characteristics include: 

    Continuous daily operation 

    Predictable business-hour peaks 

    Long-running API processes 

    Stable compute requirements 

These characteristics align well with container-based hosting. 

 

Future Growth 

The platform must support future expansion to: 

    Additional terminals 

    Additional Smart Gate integrations 

    Increased operational reporting 

    New API consumers 

Kubernetes provides a straightforward scaling model without requiring architectural redesign. 

 

Alternatives Considered 

Option 1: AWS Lambda and API Gateway 

Description 

Deploy the API as serverless functions using AWS Lambda fronted by API Gateway. 

Advantages 

    No infrastructure management 

    Automatic scaling 

    Consumption-based pricing 

    Fast initial deployment 

Disadvantages 

    Diverges from DFDS hosting standards 

    Introduces a second hosting model to support 

    Requires different operational tooling 

    Increased architectural inconsistency across platforms 

    Reduced reuse of existing Kubernetes expertise 

Decision 

Rejected. 

The organisational cost of introducing an alternative hosting model outweighs the benefits. 

 

Option 2: Amazon ECS/Fargate 

Description 

Deploy containers using AWS-managed container services. 

Advantages 

    Simplified container operations 

    Reduced infrastructure management 

    AWS-native service integrations 

Disadvantages 

    Not aligned with DFDS standards 

    Requires separate deployment patterns 

    Creates additional operational knowledge requirements 

Decision 

Rejected. 

Provides limited business value over the existing Kubernetes platform. 


Consequences 

Positive 

    Aligns with DFDS engineering standards 

    Consistent deployment approach across teams 

    Reuse of existing CI/CD pipelines 

    Reuse of existing observability tooling 

    Supports horizontal scaling 

    Supports rolling deployments 

    Supports high availability requirements 

    Reduces onboarding complexity for future teams 

 

Negative 

    Requires Kubernetes operational knowledge 

    Slightly higher infrastructure footprint compared with serverless workloads 

    Cluster management remains an organisational responsibility 

 

Operational Considerations 

Availability 

Deploy multiple replicas across availability zones. 

1     Minimum Replicas: 2 

2     Preferred Replicas: 3+ 

This ensures the service can satisfy the required: 

1     99.95% Availability 

 

Scalability 

Horizontal Pod Autoscaling (HPA) should be enabled. 

Scaling signals may include: 

    CPU utilisation 

    Memory utilisation 

    Request throughput 

    Custom application metrics 

 

Health Monitoring 

The service must expose: 

1     /health/live 

2     /health/ready 

for Kubernetes liveness and readiness probes. 

 

Security 

Secrets must not be stored within source control. 

Recommended approaches: 

1     AWS Secrets Manager 

2     External Secrets Operator 

3     Kubernetes Secrets 

according to DFDS platform standards. 

 

Observability 

All deployments must support: 

    Structured logging 

    Correlation IDs 

    OpenTelemetry tracing 

    Metrics collection 

    Distributed tracing 

    Centralised dashboards 

 

Risks 

Risk: Platform Dependency 

The solution becomes dependent on the DFDS Kubernetes platform. 

Mitigation 

Adhere to standard Kubernetes capabilities and avoid platform-specific customisations wherever possible. 

 

Risk: Resource Misconfiguration 

Improper pod sizing may impact throughput or operational costs. 

Mitigation 

Conduct performance testing and establish resource baselines before production launch. 
 

Architecture Principle Established 

All Truck Visit Management APIs will be deployed as containerized workloads on the DFDS Kubernetes platform. Alternative hosting technologies may be reconsidered in the future only where a clear business, operational, or financial benefit can be demonstrated. 