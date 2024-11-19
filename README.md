# Demo English School

## Entities

![Data Entities](/WebAPI/images/image.png)

## Route List:

```
GET/api/users - all users
GET/api/users/{id} - a selected user
POST/api/users - add user
PUT/api/users/{id} - update user
DELETE/api/users/{id} - delete user

GET/api/students - all students
GET/api/students/{id} - a selected student
POST/api/students - add student
PUT/api/students/{id} - update student
DELETE/api/students/{id} - delete student

GET/api/teachers - all teachers
GET/api/teachers/{id} - a selected teacher
POST/api/teachers - add teacher
PUT/api/teachers/{id} - update teacher
DELETE/api/teachers/{id} - delete teacher

GET/api/admins - all admins
GET/api/admins/{id} - a selected admin
POST/api/admins - add admin
PUT/api/admins/{id} - update admin
DELETE/api/admins/{id} - delete admin
```

Interface for visualisation and working with the implemented REST API is **Swagger**

## Database Configuration

This project uses a SQL Server database to store and manage data for users, students, teachers, and admins.

Connection String: "DemoEnglishSchoolDb": "Server=(localdb)\\mssqllocaldb;Database=Context-4e9e921b-801a-43f4-92a5-4629ade7b72c;Trusted_Connection=True;MultipleActiveResultSets=true"