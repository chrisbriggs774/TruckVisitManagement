# Domain Vision, Assumptions & Constraints — Truck Visit Management Domain

| | |
|---|---|
| **Version** | 1.0 |
| **Status** | Draft |
| **Author** | Solution Architecture |
| **Target Audience** | Product Owners, Domain Experts, Architects, Developers, QA Engineers |

---

## Table of Contents

1. [Purpose](#1-purpose)
2. [Domain Vision](#2-domain-vision)
3. [Business Drivers](#3-business-drivers)
4. [Domain Scope](#4-domain-scope)
5. [Primary Operating Model](#5-primary-operating-model)
6. [Architectural Assumptions](#6-architectural-assumptions)
7. [Business Constraints](#7-business-constraints)
8. [Technical Constraints](#8-technical-constraints)
9. [Non-Functional Constraints](#9-non-functional-constraints)
10. [Domain Principles](#10-domain-principles)
11. [Success Criteria](#11-success-criteria)
12. [Key Decision Record](#key-decision-record)

---

## 1. Purpose

This document defines the strategic vision, assumptions, constraints, and guiding principles for the **Truck Visit Management** domain.

Its purpose is to establish a common understanding of:

- Why the solution exists
- What business outcomes it supports
- The assumptions under which the solution is being designed
- Constraints that must be respected throughout delivery
- Design principles that should guide future architectural decisions

> This document serves as the foundation for all subsequent DDD artefacts.

---

## 2. Domain Vision

### Vision Statement

> Provide a secure, automated, auditable, and scalable truck visit management capability that enables drivers to self-manage terminal visits while providing terminal operations with real-time visibility of vehicle movements and maintaining full regulatory auditability.

### Strategic Objectives

The solution should:

| Objective | Description |
|---|---|
| **Improve Operational Efficiency** | Reduce manual gate-processing activities by enabling drivers to perform self-service interactions wherever possible. |
| **Improve Terminal Visibility** | Provide near real-time awareness of truck location and status throughout the terminal visit lifecycle. |
| **Improve Audit Compliance** | Maintain a complete and immutable history of all visit status changes for regulatory and operational review. |
| **Support Business Growth** | Enable future onboarding of additional terminals without requiring significant redesign of the core platform. |
| **Increase Automation** | Minimise reliance on gate operators by supporting automated truck processing at terminal entry and exit points. |

---

## 3. Business Drivers

The primary business drivers for the solution are:

### Automated Gate Processing
The organisation is adopting smart gate technology to reduce manual intervention and improve throughput.

### Operational Transparency
Terminal operations teams require visibility of:
- Arriving trucks
- Trucks awaiting access
- Trucks currently on site
- Completed visits

### Regulatory Compliance
The organisation is subject to regular audits and must maintain historical records of truck movements and operational decisions.

### Multi-Terminal Expansion
The platform should become a reusable capability that can be deployed across multiple terminals.

---

## 4. Domain Scope

### In Scope

The Truck Visit Management domain is responsible for:

| Area | Capabilities |
|---|---|
| **Visit Management** | Creating visits, retrieving visits, searching visits, managing visit lifecycle status |
| **Driver Information** | Capturing driver identity information, associating a driver with a visit |
| **Vehicle Information** | Capturing truck information, capturing trailer information, validating vehicle identifiers |
| **Movement Information** | Collections, deliveries |
| **Status Management** | Visit lifecycle progression, status tracking, audit history |
| **Authorization** | Terminal-based data access controls |

### Out of Scope

The following capabilities are considered external systems or future concerns:

| Area | Excluded Capabilities |
|---|---|
| **Identity Management** | User registration, user lifecycle management, password management |
| **Physical Gate Control** | Barrier control, ANPR systems, camera systems, traffic lights |
| **Terminal Operations** | Yard planning, resource scheduling, inventory management |
| **Billing** | Customer charging, invoicing, financial reconciliation |
| **Regulatory Reporting** | The domain stores auditable data but does not generate regulatory submissions |

---

## 5. Primary Operating Model

### Core Assumption

> The terminal gate operates as an automated self-service experience.

### Driver-Led Interaction

Drivers are the primary operational actor. Drivers are expected to:

- Identify themselves
- Provide vehicle information
- Provide trailer information
- Interact with Smart Gate systems
- Progress through automated workflows

> The system should assume a successful visit can be completed without human intervention.

### Gate Operator Role

Gate Operators are **not** the primary actors. Their role is limited to:

- Assisting drivers
- Handling exceptions
- Resolving validation failures
- Supporting operational incidents

**Examples**
- Driver cannot authenticate
- Driver entered incorrect vehicle details
- Hardware integration failure
- Identity verification failure
- Visit requires manual review

---

## 6. Architectural Assumptions

The following assumptions have been agreed for solution design.

| ID | Assumption | Description |
|---|---|---|
| **AA-001** | Automated Gate Environment | Terminal entry and exit are managed through Smart Gate technology. The platform is designed for an automated environment rather than a fully staffed gatehouse model. |
| **AA-002** | Driver Self-Service | Drivers will provide the majority of visit information directly, including vehicle details, trailer details, and driver identification. |
| **AA-003** | Reliable Network Connectivity | Smart Gate infrastructure is assumed to have persistent network access to backend systems. Short-term outages may occur but are considered exceptional events. |
| **AA-004** | External Authentication Provider | Authentication is delegated to a dedicated identity platform using OAuth2/OIDC standards (e.g. Microsoft Entra ID, Auth0, Okta). |
| **AA-005** | Terminal Segregation | All visit data belongs to a single terminal. Users may only access data for authorised terminals. |
| **AA-006** | Status Progression Is Sequential | Visit statuses follow a predefined lifecycle. Status regression is not permitted. |
| **AA-007** | Audit Records Are Immutable | Historical status transitions are never modified or deleted during their retention period. |
| **AA-008** | Cloud-Ready Deployment | The platform is expected to operate in a cloud-hosted environment. This should influence architecture decisions regarding scalability, observability, resilience, and security. |

---

## 7. Business Constraints

| ID | Constraint | Description |
|---|---|---|
| **BC-001** | Seven-Year Retention | Visit and audit data must be retained for at least seven years. |
| **BC-002** | Auditability | Every status transition must be traceable. The following information must be retained: previous status, new status, timestamp, originating actor. |
| **BC-003** | Near Real-Time Visibility | Operational users require visibility of visit changes shortly after they occur. |
| **BC-004** | Multi-Terminal Support | The solution must support future onboarding of additional terminals without fundamental architectural changes. |

---

## 8. Technical Constraints

| ID | Constraint | Description |
|---|---|---|
| **TC-001** | Platform Technology | The solution will be developed using the latest stable version of .NET. |
| **TC-002** | API-First Integration | Truck Visit Management capabilities are exposed through APIs. External consumers should not access persistence stores directly. |
| **TC-003** | OAuth2/JWT Security Model | All API access requires authenticated and authorised requests. |
| **TC-004** | TLS Enforcement | All network communication must use encrypted transport. |
| **TC-005** | Structured Observability | The application must support structured logging, correlation identifiers, metrics, and health monitoring. |

---

## 9. Non-Functional Constraints

| Attribute | Target |
|---|---|
| **Availability** | 99.95% |
| **Throughput** | 20,000 visits/day |
| **Peak Load** | 300 requests/second |
| **Scalability** | Must scale horizontally without redesign |
| **Security** | Must conform to enterprise security standards |

---

## 10. Domain Principles

| ID | Principle | Description |
|---|---|---|
| **DP-001** | Automation First | Automated processing should always be the preferred workflow. Manual intervention should only occur when automation fails. |
| **DP-002** | Audit by Design | Auditability is not an optional feature. All business-critical state changes must be traceable. |
| **DP-003** | Secure by Default | Security controls should be built into the solution rather than added retrospectively. |
| **DP-004** | Terminal Isolation | Terminal data must remain logically separated. Access is governed by terminal permissions. |
| **DP-005** | Business Rules Belong in the Domain | Business validation and lifecycle rules belong within the domain model. They must not be distributed across controllers, databases, or UI applications. |
| **DP-006** | Immutable History | Historical facts should never be rewritten. Corrections are represented as new events or records rather than modifications to existing audit data. |

---

## 11. Success Criteria

The solution will be considered successful if it:

- Enables self-service truck processing
- Reduces dependence on gate operators
- Provides near real-time operational visibility
- Maintains complete auditability
- Supports multiple terminals
- Meets security requirements
- Meets availability targets
- Supports future expansion without architectural redesign

---

## Key Decision Record

### Strategic Decision

> The Truck Visit Management domain is designed around an unmanned, automated Smart Gate model.

### Architectural Impact

This decision influences:

- Domain terminology
- User journeys
- Security requirements
- Exception handling workflows
- API design
- Future integrations

> The **Driver** is therefore considered the primary operational actor, while the **Gate Operator** is considered a secondary support actor responsible for exception handling and assisted processing only.
