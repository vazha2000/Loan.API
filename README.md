# ASP.NET Core Web API Application

## Introduction
This repository houses an ASP.NET Core Web API application designed for managing user accounts and loans. The application is built to provide 
comprehensive functionality, allowing accountants (administrators) to effectively manage loans and users within the system.

## Functionality 
### Accountant
- Accountants possess extensive privileges, enabling them to:
  - View, modify, and delete loans of any user, irrespective of the loan status.
  - Exercise authority to block/unblock users or loans as needed.
  - Retrieve all loans from the database and apply filters to streamline the process.

### User
- Users can leverage a range of functionalities, including:
  - Registration and authorization facilitated through JWT token authentication.
  - Access to retrieve their personal information exclusively.
  - Restriction on users listed in the black list from applying for loans.
  - Capability to add, update, and delete their loans, with the exception of loans with a status of "pending".
  - Ability to retrieve and manage their own loans exclusively, without access to other users' information.
  - Empowerment to submit loan requests with an initial status of "in process".

### Loan
- The loan application form encompasses essential details such as loan type (e.g., quick loan, car loan, installment), amount, currency (supporting GEL and USD), duration, and status (comprising options like in process, approved, rejected).
- Unauthorized users are restricted from making any modifications to loan-related data.

## Technologies Used
- ASP.NET Core Web API (.NET 6)
- Entity Framework Core
- MS SQL for data storage
- JWT for authorization
- FluentValidation.AspNetCore for input validation

## Getting Started
To clone the repository, use the following command:
**git clone https://github.com/vazha2000/Loan.API**

Once cloned, navigate to the project directory and follow the steps below to set up and run the application.


 **Install dependencies:**
  
 **Set up the database:**
 - add-migration [migration name]
 - update-database


