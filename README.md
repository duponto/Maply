Maply is a minimal, convention-based object mapper for .NET.

It focuses on high-performance property-to-property mapping
when advanced configuration is not required.

## Features

- Zero configuration
- Expression tree compilation
- Cached delegates
- Thread-safe
- Lightweight

## When to use

Maply is ideal for:

- Simple DTO transformations
- Internal services
- High-throughput scenarios
- Microservices with minimal mapping rules

## Limitations & Future Investigation

Maply intentionally focuses on simple, convention-based mapping.

The following capabilities are currently not supported and are being evaluated for potential future versions:

- Nested object mapping
- Custom property resolvers
- Conditional mapping
- Projection support (IQueryable scenarios)
- Advanced transformation pipelines

The goal is to evolve carefully without compromising the library's lightweight and performance-oriented design.
