
# BMI Calculator

BMI Calculator is a demonstration of a distributed .NET microservices application, utilizing Docker for orchestration. This project highlights the use of asynchronous programming, fault isolation via the Polly library, and demonstrates the effectiveness of microservices in handling complex tasks. The application consists of two microservices: "Collector" for data gathering and validation, and "Calculator" for BMI calculation based on height and weight, determining BMI status (e.g., Overweight, Underweight, etc.). It also features a GUI (MVC) for displaying the calculator interface.

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![Microservices](https://img.shields.io/badge/Microservices-0078D6?style=for-the-badge&logo=microservices&logoColor=white) ![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white) ![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white) ![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black) ![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white)

---

### Key Features

- **Asynchronous Communication**: Seamless interaction between the Collector and Calculator services.
- **Fault Isolation**: Incorporation of the Polly library for robust retry patterns, ensuring resilience.
- **Logging**: Implementation of Seq for efficient tracking and visualization of logs.
- **User Interface**: An MVC architecture-based service that serves as the domain interacting with the Collector service.

---

### Components

- **Collector Service**: Gathers and validates user data for BMI calculation.
- **Calculator Service**: Computes the BMI based on height and weight and determines the corresponding health status.
- **User Interface**: A front-end solution presenting a user-friendly BMI calculator.
- **Monitoring**: Includes logging mechanisms for tracking and analysis.

---

### Technologies

- **.NET**: Utilized for building microservices and the UI.
- **Docker**: Employed for containerization and orchestration of services.
- **Polly**: Integrated for implementing resilience patterns.
- **Seq**: Used for visualizing and managing logs.

---

### Installation and Setup

1. **Install Dependencies**:
   - **Collector**: Newtonsoft.json, Polly, Microsoft.Extensions.Http.Polly
   - **Calculator**: No additional dependencies.
   - **Monitoring**: Serilog, Serilog.Enrichers.Span, Serilog.Sinks.Seq, Serilog.Sinks.Console
   - **UI**: Polly, Microsoft.Extensions.Http.Polly, Newtonsoft.json
2. **Run**: Execute `docker compose up –build ui-service` to start the application.
3. **Access**:
   - **UI**: [localhost:8000](http://localhost:8000)
   - **Seq**: [localhost:5342](http://localhost:5342)

---

### Contributing

Contributions are welcome. Special thanks to Wehba Korouni for the cooperation. This project is maintained by Said Mansour.

---

### Contact

For any inquiries or contributions, please open an issue on the project's [GitHub page](#).

---

