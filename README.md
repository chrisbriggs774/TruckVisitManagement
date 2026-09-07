Product Backlog Item: Truck Visit Management API
Scenario:
Gate management at one of our biggest terminals has requested a new feature for the 'Smart Gate' solution, which will allow them to track truck visits to the terminal. This feature requires an API with endpoints for creating and reading visit records. The API will handle information about the truck, the driver, and a list of collections and deliveries.
Acceptance Criteria:
1. Business Requirements:
•
Gate operators need near real-time visibility of truck status
•
Regulatory audits occur regularly
•
The solution should support future expansion to multiple terminals without significant redesign.
2. Operational Requirements:
•
Up to 20, 000 truck visits per day
•
Peaks of 300 requests per second
•
99.95% availability
•
Full audit history of status changes
•
Retention period of 7 years
3. Architectural Requirements:
•
High-level architecture diagram
•
Logical component and module boundaries
•
API design
•
Data storage rationale
•
Security Model
4. Technical Requirements:
•
Create an endpoint for adding a visit record.
•
Create an endpoint for retrieving a stored visit record by Id.
•
Create an endpoint for searching visits using the following query params:
o
terminalId
o
currentStatus
o
movementFrom
o
movementTo
o
createdTimeFrom
o
createdTimeTo
o
createdBy
o
page
o
pageSize
•
A visits current status may be updated, however all status transitions must be retained as an immutable audit history.
•
Status values are: Pre-Registered, At Gate, On Site, Completed
•
The visit record should include details about the truck, the driver, and a list of collections and deliveries.
•
Ensure unit numbers and license plates are capitalized and contain no whitespaces.
•
Use the latest stable version of .NET.
•
Ensure all business logic is covered by unit tests. Additional integration tests may be included where they provide confidence in key application behaviours (e.g. persistence, authentication, or audit history).
5. Security Requirements:
•
OAuth2/JWT Bearer authentication
•
Authorization based on terminal access
•
TLS enforced
•
Input validation
•
Security headers
•
Audit logging
•
Secrets managed outside source control
6. Observability Requirements:
•
Structured logging
•
Correlation IDs
•
Metrics for API throughput and latency
•
Audit logging of status changes
•
Health check endpoint
•
Error tracking strategy
7. Bonus (Optional)
This section is optional and an actual deployment is not expected. Feel free to pick any of the topics below and briefly explain your approach in your architecture document:
•
Containerizing the application with Docker
•
How you would deploy it to Kubernetes (a sample manifest is enough, no cluster needed)
•
How you would use AWS services (e.g. RDS, CloudWatch)
Tasks:
1. Design API Endpoints
Description: Design the API contract for creating and retrieving visit records.
Acceptance Criteria: API contracts for create and read operations are defined.
2. Implement Create Visit Endpoint
POST /api/visits
Description: Implement the endpoint to create a visit record.
Acceptance Criteria: The endpoint accepts visit details and stores them.
3. Implement Read Visit Endpoint
GET /api/visits/{id}
Description: Implement the endpoint to retrieve
a visit by its Id.
Acceptance Criteria: The endpoint returns
the stored visit record when found. A suitable response is returned when the record does not exist.
4. Implement Search Visits Endpoint
GET /api/visits
Description: Implement
the endpoint that allows visit records to be searched and filtered.
Acceptance Criteria:
The endpoint supports filtering using documented query parameters, supports pagination, returns matching visit records and returns paging metadata where appropriate.
5. Data Storage Setup
Description: Set up the data storage for visit records.
Acceptance Criteria: A working data storage solution is in place.
6. Security Implementation
Description: Implement security measures to protect API endpoints.
Acceptance Criteria: API endpoints are secured
in a manner consistent with the documented security requirements.
7. Unit Testing
Description: Write unit tests for business logic.
Acceptance Criteria: All business logic is covered by unit tests.
8. Code Review and Refactoring
Description: Review and refactor code to ensure best practices.
Example Domain Overview:
Visit:
Id
Terminal Id
CurrentStatus
Truck: {}
Driver: {}
Movements[]
StatusHistory[]
CreatedTime
CreatedBy
•
Truck information must include at least a unit number and license plate.
•
Driver information must be captured.
•
Movements must support both collections and deliveries.
•
Status history must support a complete immutable audit trail.
•
Candidates may model and persist the data in any manner they feel is appropriate
Assumptions and Trade-Offs:
The submission should include a short document covering:
•
Assumptions made due to incomplete or ambiguous requirements
•
Architectural decisions and rationale
•
Alternative approaches considered
•
Trade-offs accepted (e.g. simplicity vs scalability etc)
•
Known limitations and future improvements
Submission Requirements:
Please provide:
•
Source code in a GitHub repository
•
Commit history showing progression of work
•
README containing setup and execution instructions, including tooling requirements
•
Architecture documentation
•
Assumptions and trade-offs document
•
Test execution instructions
Assessment Areas:
Candidates will be assessed on:
•
Code quality and maintainability
•
Architectural reasoning
•
Scalability considerations
•
Security design
•
Test strategy
•
Domain modelling
•
Operational readiness
•
Communication of trade-offs
Expected Effort:
Timeframe: Two weeks from receipt.
Expected effort: Approximately 10-15 hours.
This exercise is intentionally open-ended. It is not expected that every possible enhancement will be implemented. Where time constraints prevent implementation, candidates should document the intended approach and rationale.
We are more interested in architectural decision making, prioritisation, trade-offs, communication and code quality than a feature-complete solution.
