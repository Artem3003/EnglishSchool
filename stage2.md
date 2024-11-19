# Working with persistent storage, validation of input data

## Objective.

To gain skills in working with ORM technology for permanent storage of REST API entities in the database, as well as the use of design templates “Repository” and ‘Data Transfer Object’ design templates.

## Tasks.

1. Replace the software developed in the course of the the software developed during the first computer workshop, from temporary data storage to permanent data storage in the database.

2. Add validation of input data. 

## Software requirements.

1. To implement the software in the course of the computer any stack (programming language, web framework, etc.) of technologies.

2. Any database can be used for permanent storage of entities database.

3. To work with the database, you need to use ORM and the design template “Repository”.

4. To get repository instances for working with the database, you need to use an IoC container for dependency injection.

5. The REST API, instead of passing and receiving entity objects stored in the database in the database, should use separate DTOs.

6. Input DTOs must be validated for correctness (using the FluentValidation library or an analog for the selected technology stack).