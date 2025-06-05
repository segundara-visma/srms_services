# School Record Management System

## (Microservices Application - Clean Architecture - .NET)

[![github-shield]][github-url]
[![Issues][issues-shield]][issues-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

---

## Overview[![](./src/assets/images/pin.jpg)](#introduction)

This project is a microservices-based application built using .NET with Clean Architecture principles. Each service is independent, exposes HTTP endpoints, and follows a layered structure: **Presentation → Application → Domain → Infrastructure**.

---

## Prerequisites[![](./src/assets/images/pin.jpg)](#prerequisites)
To run this project, ensure you have the following installed:
* **.NET SDK v9.0**: This project uses .NET v9.0 for development and runtime.
* **Docker**: Required for containerizing and running the services.
* **Docker Compose**: Used to manage multi-container deployments.
* **PostgreSQL**: Each service requires a PostgreSQL database.
* **Redis**: Used for token management and caching.

---

## Services[![](./src/assets/images/pin.jpg)](#pre-commit-hook)

The application currently consists of the following services:
* **Auth Service**: Handles JWT authentication, refresh tokens, and uses Redis for token management.
* **User Service**: Manages user-related operations.
* **Course Service**: Handles course-related functionality.
* **Student Service**: Manages student data and operations.
* **Enrollment Service**: Manages student enrollments in courses.
* **Grade Service**: Handles grading and assessment.
* **Report Service**: Generates reports/transcripts for students.
* **Tutor Service**: Manages tutor-related functionality.
* **Admin Service**: Provides administrative capabilities.

---

## Architecture[![](./src/assets/images/pin.jpg)](#pre-commit-hook)
* **Clean Architecture**: Each service follows the Clean Architecture pattern with distinct layers.
* **HTTP Endpoints**: Services expose RESTful APIs for interaction.
* **Database-per-Service**: Each service has its own PostgreSQL database.

---

## Authentication[![](./src/assets/images/pin.jpg)](#pre-commit-hook)
* **JWT and Refresh Tokens**: Clients obtain tokens from the Auth Service.
* **Token Validation**: All services validate tokens using a shared Redis cache.
* **Token Invalidation**: Managed by the Auth Service, with updates pushed to Redis.
* **Service-to-Service Auth**: Uses Auth0 for internal token-based authentication.

---

## Inter-service Communication[![](./src/assets/images/pin.jpg)](#pre-commit-hook)
* Services communicate via HTTP clients (no API gateway or message bus currently).

---

## Data Layer[![](./src/assets/images/pin.jpg)](#pre-commit-hook)
* **PostgreSQL**: Each service has its own dedicated database.
* **Redis**: Used for token blacklist checks and planned for future caching.

---

## Deployment[![](./src/assets/images/pin.jpg)](#pre-commit-hook)
* **Docker**: All services run inside Docker containers.
* **CI/CD**: Automated via GitHub Actions for building, testing, and pushing Docker images.

---

## Testing[![](./src/assets/images/pin.jpg)](#pre-commit-hook)
* **TDD Approach**: Tests are written using:
  * **xUnit**: For unit testing.
  * **Moq**: For mocking dependencies.
  * **FluentAssertions**: For readable assertions.

---

## Planned Features[![](./src/assets/images/pin.jpg)](#pre-commit-hook)
* **API Gateway**: To be implemented using YARP or Ocelot.
* **Frontend**: A web app is planned but not yet specified.
* **Logging and Service Discovery**: To be added in future iterations.

---

## Getting Started[![](./src/assets/images/pin.jpg)](#pre-commit-hook)
* Clone the repository:
```
git clone https://github.com/segundara-visma/srms_services.git
```
* Ensure Docker and .NET SDK are installed.
* Build and run services using Docker Compose:
```
docker-compose up --build
```
* Access the services via their respective HTTP endpoints.

---

[![](./src/assets/images/backtotop.png)](#school-record-management-system)

[issues-shield]: https://img.shields.io/github/issues/segundara-visma/srms_services.svg?style=flat-square
[issues-url]: https://github.com/segundara-visma/srms_services/issues
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=flat-square&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/olusegunemmanuelokedara/
[github-shield]: https://img.shields.io/static/v1?label=Version%20control&message=Github&color=blue
[github-url]: https://github.com/segundara-visma/srms_services