# wolverine-saga-repro

This is a reproduction of [Wolverine issue #1235](https://github.com/JasperFx/wolverine/issues/1235).

## Dependencies and Setup

Integration tests use [Testcontainers](https://testcontainers.com/), so Docker
must be running for tests to run.

Otherwise, a PostgreSQL database named `saga-repro` (by default) must exist and
be reachable by the application. Modify the
[connection string](https://github.com/esond/wolverine-saga-repro/blob/44c1d6f0f3c64f41baba0765ebb2fee65d47c99b/src/Api/appsettings.Development.json#L9)
as needed.

## Reproducing the issue

Run tests either from your IDE or using `dotnet test`. The [saga](https://github.com/esond/wolverine-saga-repro/blob/main/src/Api/Order.cs)
in this example has `required` properties that can be commented out in favour
of the other commented out properties to view the differences in behaviour in
those scenarios described in the issue.
