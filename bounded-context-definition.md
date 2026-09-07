Bounded Context Definition 

Truck Visit Management Domain 

Version: 1.0 
Status: Draft 
Document Type: Domain-Driven Design (DDD) 
Audience: Product Owners, Architects, Developers, Business Analysts, QA Engineers 

 

1. Purpose 

This document defines the bounded contexts within the Truck Visit Management domain and the relationships between them. 

The objective is to: 

    Establish clear ownership boundaries 

    Minimise coupling 

    Define responsibility for business capabilities 

    Enable future expansion without architectural redesign 

    Create alignment between business domains and software architecture 

 

2. Context Definition Approach 

When identifying bounded contexts, the following principles have been applied: 

Business Capability Alignment 

A bounded context should map to a distinct business capability. 

High Cohesion 

Concepts that frequently change together should exist within the same context. 

Low Coupling 

Contexts should depend on each other through well-defined contracts. 

Future Scalability 

The design must support future terminal expansion and potential system decomposition. 

 

3. Domain Landscape 

The Truck Visit Management solution operates within a broader Smart Gate ecosystem. 

1     +------------------------------------------------+ 

2     |              Terminal Operations               | 

3     +------------------------------------------------+ 

4                          | 

5                          | 

6       +--------------------------------------------+ 

7       |     Truck Visit Management Domain          | 

8       +--------------------------------------------+ 

9           |             |             | 

10           |             |             | 

11           v             v             v 

12      Driver Identity  Smart Gate   Audit & 

13      Verification     Processing   Compliance 

14      

 

4. Identified Bounded Contexts 

The following contexts have been identified. 

 

4.1 Visit Management Context 

Classification 

Core Domain 

This is the primary business domain. 

 

Purpose 

Manage the lifecycle of truck visits from registration through completion. 

 

Responsibilities 

    Create Visit 

    Store Visit 

    Search Visit 

    Update Visit Status 

    Maintain Visit Lifecycle 

    Manage Collections 

    Manage Deliveries 

    Manage Truck Information 

    Manage Trailer Information 

    Manage Driver References 

 

Owns 

Entities 

1     Visit 

2     Truck 

3     Trailer 

4     DriverReference 

5     Collection 

6     Delivery 

Value Objects 

1     TerminalId 

2     LicensePlate 

3     TruckUnitNumber 

4     TrailerNumber 

5     VisitId 

Business Rules 

1     Status transitions 

2     Vehicle normalization 

3     Terminal ownership 

4     Visit lifecycle validation 

 

Ubiquitous Language 

Terms owned by this context: 

1     Visit 

2     Movement 

3     Collection 

4     Delivery 

5     Truck 

6     Trailer 

7     Status 

8     Check-In 

9     Check-Out 

 

Why It Exists 

This is the reason the system exists. 

Without Visit Management there is no business capability. 

 

4.2 Driver Verification Context 

Classification 

Supporting Domain 

 

Purpose 

Verify that a Driver is authorised and identifiable. 

 

Responsibilities 

    Validate identity 

    Verify credentials 

    Maintain driver verification outcome 

    Record verification events 

 

Owns 

Entities 

1     Driver Identity 

2     Verification Result 

3     Verification Session 

 

Ubiquitous Language 

Terms owned: 

1     Driver Identity 

2     Verification 

3     Identity Document 

4     Verification Outcome 

 

Notes 

The Visit Management context should not contain identity verification logic. 

Instead: 

1     Visit Management requests verification 

2      

3     Driver Verification returns outcome 

This reduces complexity and supports future integration with external identity providers. 

 

4.3 Terminal Access Context 

Classification 

Supporting Domain 

 

Purpose 

Control who can access terminal data. 

 

Responsibilities 

    Terminal permissions 

    Terminal membership 

    Claims interpretation 

    Authorization decisions 

 

Owns 

Concepts 

1     Terminal Access 

2     Permission 

3     Role 

4     Authorization Policy 

 

Business Rules 

1     Users can only access authorised terminals. 

2     Users cannot retrieve visits from 

3     unauthorised terminals. 

 

Ubiquitous Language 

Terms owned: 

1     Terminal Access 

2     Authorization 

3     Permission 

4     Role 

 

Notes 

This keeps security concerns separate from operational concerns. 

 

4.4 Audit & Compliance Context 

Classification 

Generic Supporting Domain 

 

Purpose 

Provide immutable historical records. 

 

Responsibilities 

    Status history 

    Audit storage 

    Audit retrieval 

    Compliance reporting support 

 

Owns 

Entities 

1     Audit Record 

2     Audit Trail 

3     Status History 

 

Business Rules 

1     Audit records are immutable. 

2     Audit records cannot be deleted 

3     before retention expiry. 

 

Ubiquitous Language 

Terms owned: 

1     Audit Record 

2     Audit Trail 

3     Status History 

4     Retention 

 

Notes 

The source of truth for the current visit state remains Visit Management. 

Audit Context owns historical facts. 

 

4.5 Smart Gate Interaction Context 

Classification 

Supporting Domain 

 

Purpose 

Manage interactions between automated gate systems and drivers. 

 

Key Assumption 

The terminal gate is unmanned and automated. 

Drivers interact directly with Smart Gate systems. 

 

Responsibilities 

    Driver sessions 

    Gate workflows 

    Guided user journeys 

    Device interaction 

    Exception routing 

 

Owns 

Entities 

1     Gate Session 

2     Driver Session 

3     Interaction Workflow 

4     Exception Case 

 

Ubiquitous Language 

Terms owned: 

1     Session 

2     Check-In Journey 

3     Check-Out Journey 

4     Exception 

5     Assisted Processing 

 

Notes 

This context exists because the business has explicitly chosen an automation-first operating model. 

Without this context: 

1     Visit Management would become tightly 

2     coupled to gate workflow behaviour. 

 

5. Context Relationships 

1                         +--------------------+ 

2                         | Terminal Access    | 

3                         +---------+----------+ 

4                                   | 

5                                   | 

6                                   v 

7      

8     +----------------+    +-------------------+    +-------------------+ 

9     | Driver         |--->| Visit Management  |--->| Audit & Compliance| 

10     | Verification   |    |  (Core Domain)    |    +-------------------+ 

11     +----------------+    +-------------------+ 

12                                   ^ 

13                                   | 

14                                   | 

15                         +---------+----------+ 

16                         | Smart Gate         | 

17                         | Interaction        | 

18                         +--------------------+ 

 

6. Context Ownership Matrix 

Capability 
	

Context Owner 

Visit lifecycle 
	

Visit Management 

Collections 
	

Visit Management 

Deliveries 
	

Visit Management 

Truck details 
	

Visit Management 

Trailer details 
	

Visit Management 

Driver identity verification 
	

Driver Verification 

Terminal permissions 
	

Terminal Access 

Audit history 
	

Audit & Compliance 

Driver check-in workflows 
	

Smart Gate Interaction 

Driver check-out workflows 
	

Smart Gate Interaction 

Exception handling 
	

Smart Gate Interaction 

 

7. Strategic Context Classification 

Core Domain 

1     Visit Management 

Competitive advantage exists here. 

This receives the highest engineering investment. 

 

Supporting Domains 

1     Driver Verification 

2     Terminal Access 

3     Smart Gate Interaction 

These support operational workflows. 

 

Generic Domains 

1     Audit & Compliance 

2     Authentication Provider 

3     Observability Platform 

These are necessary but not unique business differentiators. 

 

8. Recommended Initial Implementation 

Although multiple bounded contexts exist conceptually, the first implementation should remain a single deployable solution. 

Recommended Structure 

1     TruckVisitManagement 

2      

3     ├── VisitManagement 

4     ├── DriverVerification 

5     ├── TerminalAccess 

6     ├── AuditCompliance 

7     ├── SmartGateInteraction 

8     └── SharedKernel 

 

Why Not Microservices? 

Current requirements: 

1     20,000 visits per day 

2     300 requests per second 

do not justify operational complexity associated with distributed systems. 

Bounded Contexts define business ownership. 

They do not automatically imply separate deployments. 

 

9. Anti-Corruption Principles 

External systems must not directly influence the internal domain model. 

Adapters should be created for: 

1     Identity Providers 

2     ANPR Systems 

3     Gate Hardware 

4     Terminal Operating Systems 

5     Future ERP Systems 

This protects the domain model from external changes. 

 

10. Future Evolution Roadmap 

If future scale or organisational boundaries require decomposition: 

First Candidate 

1     Audit & Compliance 

Low coupling and independently scalable. 

 

Second Candidate 

1     Driver Verification 

Can be outsourced to external providers. 

 

Third Candidate 

1     Smart Gate Interaction 

May evolve into a dedicated kiosk/device platform. 

 

Last Candidate 

1     Visit Management 

Remains the core domain and system of record. 

 

Architectural Decisions Captured 

BC-001 

Visit Management is the Core Domain and owns the Visit lifecycle. 

BC-002 

Driver identity verification is separated from Visit Management. 

BC-003 

Audit history is treated as a distinct bounded context. 

BC-004 

Terminal authorization is separated from business processing. 

BC-005 

Smart Gate interaction is modelled as a dedicated bounded context due to the automation-first operating model. 

BC-006 

Bounded contexts will initially be implemented within a modular monolith and only separated into independent services if justified by future business or scaling requirements. 
