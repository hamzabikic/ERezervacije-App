# ERezervacije Application

# Description 

ERezervacije is a web application designed for managing restaurant reservations. It is tailored for the needs of guests, hostesses, and managers, enabling easy and efficient handling of reservations and related tasks. The web application is fully localized in Bosnian, providing an intuitive experience for real-world restaurant environments.

The application is implemented with Angular for the frontend and C# ASP.NET Core Web API for the backend, connected to a SQL database. It features advanced functionalities such as uploading and sending images to the backend, sending emails via SMTP (email verification, password reset, reservation acceptance or rejection), and sending SMS messages via Twilio (phone number verification, reservation acceptance or rejection).

The code is structured in a way that centralizes all repetitive functionalities, making the application easier to maintain and extend.

<h5>Key Features</h5>

Role-Based Access Control: The system implements role-based access control, providing different levels of permissions to three main user groups: guests, hostesses, managers

Enhanced Security Measures: Stringent security measures are in place at both the frontend and backend to mitigate the risk of unauthorized access and misuse. This ensures the safety of sensitive data and prevents potential security breaches.

Real-world Usability: The application is designed to be deployed and used in real-world restaurant environments. It can seamlessly integrate into restaurant workflows, assisting guests, hostesses, and managers in efficiently managing reservations and related tasks.

<h5>Getting Started</h5>

Start the Angular project, and navigate to the login page.

Use the following credentials to access the system as a guest:

Username: gost

Password: testtest

Hostess:

Username: hostesa

Password: testtest

Manager:

Username: manager

Password: testtest

The application can also be tested at the URL: https://web.p2347.app.fit.ba/

# Instructions for Running the Web Application
Instructions for Running the Web Application
This repository contains an Angular web application developed as part of the project. Follow these steps to successfully run the application on your local machine.

Prerequisites Before running the application, make sure you have the following installed:

Node.js (version 12.x or newer)

npm (Node.js package manager, usually installed alongside Node.js)

Installation

Clone this repository to your local machine using the following command:

git clone https://github.com/hamzabikic/EStudentskaSluzba-App.git

Navigate to the application directory in your terminal:

cd repository-name

Install all the necessary dependencies by running:

npm install

Running Once you have successfully installed all the required dependencies, you can run the application using the following command:

npm start

This command starts a development server. Once the server is up and running, the application will be available at http://localhost:4200/ in your web browser.

# Instructions for Running the Web Api

Prerequisites: Before running the application, make sure you have installed the .NET Core SDK, which is required for building and running ASP.NET Core applications.

Clone the Repository: Clone the repository to your local machine using the provided URL.

Configuration: Configure your application settings in the appsettings.json file, including settings for the database, access keys, and other configuration parameters.

Database Setup: Before starting the web API, ensure that the SQL Server is installed and running on your local machine. Then, follow these steps:

- Open the NuGet Package Console in Visual Studio.
- Run the following command to add a migration (replace "name-of-migration" with a suitable migration name): add-migration name-of-migration
- After adding the migration, run the following command to update the database with the changes: update-database

Build the Application: Open a terminal or command prompt in the application directory and execute the following command to build the application:
dotnet build

Run the Application: After a successful build, run the application from the terminal or command prompt with the following command:
dotnet run

After starting the web API, the you need to manually call the "TestniPodaci/generisiTestnePodatke" endpoint. This step is necessary to initialize the database by creating basic objects needed for testing or demonstration purposes. Once the test data is generated, the application will be ready for use.

The application will start on a local server and will be accessible at http://localhost:5000 (check which port it is listening on in your application)

Testing API Endpoints: Utilize a tool like Postman or Swagger UI to test the API endpoints.

Swagger Integration: Swagger has been integrated into the application to provide interactive API documentation. Once the application is running, navigate to the Swagger endpoint (e.g., http://localhost:5000/swagger) to access the Swagger UI and explore the API endpoints.

# Screenshots

<h4>Computer</h4>
<img src="/Screenshots/login.png">
<img src="/Screenshots/registracija.png">
<img src="/Screenshots/zaboravljenaLozinka.png">
<img src="/Screenshots/moj-nalog-gost.png">
<img src="/Screenshots/kreiranje-rezervacije-gost.png">
<img src="/Screenshots/moj-nalog-hostesa.png">
<img src="/Screenshots/moj-nalog-manager-dijalog.png">
<img src="/Screenshots/upravljanje-rezervacijama-manager.png">
<img src="/Screenshots/upravljanje-gostima-manager.png">
<img src="/Screenshots/upravljanje-hostesama-manager.png">
<img src="/Screenshots/upravljanje-stolovima-manager.png">
<h4>Smartphone</h4>
<img src="/Screenshots/login-mobilni.png">
<img src="/Screenshots/moj-nalog-gost-mobilni.png">
<img src="/Screenshots/moj-nalog-gost-mobilni-2.png">
<img src="/Screenshots/navbar-mobilni.png">
<img src="/Screenshots/kreiranje-rezervacije-gost-mobilni.png">
<img src="/Screenshots/kreiranje-rezervacije-gost-mobilni-2.png">
<img src="/Screenshots/upravljanje-gostima-manager-mobilni.png">


# License
This project is protected by the "Proprietary No-Use License". See the LICENSE file for more information.





