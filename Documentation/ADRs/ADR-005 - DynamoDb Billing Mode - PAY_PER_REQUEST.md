# ADR-005: DynamoDB Billing Mode – PAY_PER_REQUEST

| | |
|---|---|
| **Status** | Accepted |

---

## Table of Contents

1. [Context](#context)
2. [Decision](#decision)
3. [Rationale](#rationale)
4. [Alternatives Considered](#alternatives-considered)
5. [Cost Considerations](#cost-considerations)
6. [Consequences](#consequences)
7. [Architecture Principle Established](#architecture-principle-established)

---

## Context

The Truck Visit Management platform uses DynamoDB as its transactional write store ([ADR-001](./adr-001-cqrs-dynamodb-opensearch.md)).

The workload characteristics are:

- Up to 20,000 truck visits per day
- Peak traffic of approximately 300 requests per second
- Event-driven architecture
- Multi-terminal growth expected
- Write-heavy workload
- Variable operational demand throughout the day

Traffic patterns are expected to fluctuate based on:

- Terminal operating hours
- Ferry schedules
- Haulier arrival patterns
- Seasonal demand
- Future terminal onboarding

The platform must support growth without requiring frequent capacity management activities.

---

## Decision

The DynamoDB tables will be configured using:

> **Billing Mode = `PAY_PER_REQUEST`**

...for all environments.

---

## Rationale

### Operational Simplicity

`PAY_PER_REQUEST` removes the need to:

- Forecast capacity
- Manage Read Capacity Units (RCUs)
- Manage Write Capacity Units (WCUs)
- Configure autoscaling policies

This reduces operational complexity and support overhead.

### Traffic Variability

Expected traffic patterns are unlikely to be perfectly predictable, for example:

- Morning gate peaks
- Ferry arrival surges
- Seasonal operational changes
- Future terminal expansion

`PAY_PER_REQUEST` automatically scales to match workload demand.

### Current Scale

The anticipated load — **20,000 visits per day**, **300 peak requests per second** — is well within DynamoDB's ability to handle on-demand scaling.

The platform is not expected to operate at a sustained throughput level where provisioned capacity would provide significant savings.

### Reduced Risk

Provisioned capacity introduces risks:

- Under-provisioning
- Throttle events
- Operational configuration errors

`PAY_PER_REQUEST` eliminates these risks.

### Future Multi-Terminal Expansion

The number of terminals supported by the solution is expected to increase over time. Using on-demand billing prevents repeated capacity reviews and reconfiguration exercises during onboarding of new terminals.

---

## Alternatives Considered

### Provisioned Capacity with Auto Scaling

| Advantages | Disadvantages |
|---|---|
| Potentially lower costs at very high sustained throughput | Capacity planning required |
| Predictable monthly billing | Risk of throttling |
| | Additional operational management |

**Decision**: Rejected. Workload characteristics do not currently justify capacity management complexity.

### Provisioned Capacity without Auto Scaling

| Advantages | Disadvantages |
|---|---|
| Lowest potential cost | Significant operational overhead |
| | High risk of throttling |
| | Poor fit for variable workloads |

**Decision**: Rejected.

---

## Cost Considerations

The primary goal is:

- Operational simplicity
- Scalability
- Reduced operational risk

...rather than minimum possible infrastructure cost.

Future cost optimisation may revisit this decision if:

- Traffic becomes predictable
- Sustained high throughput exists
- Cost analysis demonstrates savings

---

## Consequences

### Positive
- No capacity planning
- Automatic scaling
- No throttling due to misconfiguration
- Simplified operations
- Faster onboarding of new terminals

### Negative
- Less predictable monthly cost
- Potentially higher costs compared to well-managed provisioned capacity at very large scale

---

## Architecture Principle Established

> DynamoDB tables shall use `PAY_PER_REQUEST` billing to optimize for operational simplicity, workload variability, and future scalability rather than capacity-management optimisation.
