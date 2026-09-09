# ADR-006: OpenSearch Instance Sizing Strategy

| | |
|---|---|
| **Status** | Accepted |

---

## Table of Contents

1. [Context](#context)
2. [Decision](#decision)
3. [Rationale](#rationale)
4. [Environment Sizing](#environment-sizing)
5. [Future Review Criteria](#future-review-criteria)
6. [Alternatives Considered](#alternatives-considered)
7. [Consequences](#consequences)
8. [Architecture Principle Established](#architecture-principle-established)

---

## Context

[ADR-001](./adr-001-cqrs-dynamodb-opensearch.md) establishes OpenSearch as the query side of the CQRS architecture.

OpenSearch is responsible for:

- Visit Search
- Operational Filters
- Reporting Queries
- Dashboard Queries
- Future Analytics Requirements

The platform's usage profile indicates:

- Search requests occur less frequently than write operations
- Operational users search for active visits
- Query complexity may increase over time
- Read performance is important but not mission-critical
- Search data can be rebuilt from DynamoDB if required

The infrastructure must be appropriately sized while remaining cost-effective.

---

## Decision

The following OpenSearch sizing strategy will be adopted.

### Development Environment

| Setting | Value |
|---|---|
| Instance Type | `t3.small.search` |
| Node Count | 1 |
| Availability | Non-Production, no HA requirement |

**Purpose**: Supports local integration testing, functional validation, developer experimentation, and lower operational cost. A single node is considered acceptable.

### Production Environment

| Setting | Value |
|---|---|
| Instance Type | `m6g.large.search` |
| Node Count | 2 |
| Availability | 99.95% service requirement |

**Purpose**: Supports production search workloads, multi-AZ deployment, query resiliency, and future growth. Two nodes provide redundancy and support cluster resilience.

---

## Rationale

### Why OpenSearch Is Sized Conservatively

The solution architecture assumes:

> Writes > Reads

The operational workload primarily consists of:

- Visit Creation
- Status Transitions
- Audit Recording

Search is a secondary workload. Therefore infrastructure investment should prioritise DynamoDB, API availability, and projection reliability rather than aggressive OpenSearch scaling.

### Why `t3.small.search` in Development

Benefits:

- Low cost
- Fast provisioning
- Suitable for functional testing
- Minimal operational overhead

Production-level search performance is not required within development environments.

### Why `m6g.large.search` in Production

Benefits:

- Better CPU performance
- Additional memory
- ARM cost efficiency
- AWS Graviton optimisation

The instance size provides sufficient headroom for search indexing, query execution, future filter expansion, and increased terminal adoption.

### Why Two Production Nodes

Benefits:

- Node redundancy
- Rolling upgrade support
- Improved availability
- Improved failure recovery

The solution can tolerate loss of a single node while maintaining service availability.

---

## Environment Sizing

| Environment | Node Count | Instance Type |
|---|---|---|
| Development | 1 | `t3.small.search` |
| Production | 2 | `m6g.large.search` |

---

## Future Review Criteria

This decision should be revisited if:

- Search latency exceeds SLA
- Terminal count increases significantly
- Data volumes grow beyond expectations
- Reporting usage increases substantially

Potential future scaling options include:

- Increase node count
- Increase instance size
- Introduce dedicated master nodes
- Introduce warm storage tiers

---

## Alternatives Considered

### `m6g.xlarge.search`

| Advantages | Disadvantages |
|---|---|
| Greater performance headroom | Increased cost |
| | Unnecessary for initial workload |

**Decision**: Rejected.

### Three Production Nodes

| Advantages | Disadvantages |
|---|---|
| Additional resilience | Higher cost |
| | Limited business value at current scale |

**Decision**: Rejected for initial deployment. May be revisited as usage grows.

### Serverless OpenSearch

| Advantages | Disadvantages |
|---|---|
| Reduced infrastructure management | Different operational model |
| | Less control over sizing |
| | Additional platform complexity |

**Decision**: Rejected.

---

## Consequences

### Positive
- Cost-effective deployment
- Aligns with expected workload profile
- Supports future growth
- Meets availability requirements

### Negative
- Additional scaling review may be required as adoption grows
- Search capacity must still be monitored

---

## Architecture Principle Established

> OpenSearch will be sized according to its role as a secondary read model within the CQRS architecture. Development environments will run a single `t3.small.search` node, while Production will run a two-node `m6g.large.search` cluster to provide resilience, availability, and operational headroom.
