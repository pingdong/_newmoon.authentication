# Newmoon.Authentication

The repo contains the code that manages and authenticate users.

## Status

Develop Branch:<br />
[![Build Status](https://pingdong.visualstudio.com/Newmoon/_apis/build/status/pingdong.newmoon.authentication?branchName=develop)](https://pingdong.visualstudio.com/Newmoon/_build/latest?definitionId=26&branchName=develop)<br />
Master Branch:<br />
[![Build Status](https://pingdong.visualstudio.com/Newmoon/_apis/build/status/pingdong.newmoon.authentication?branchName=master)](https://pingdong.visualstudio.com/Newmoon/_build/latest?definitionId=26&branchName=master)<br />
<br />

## Init Setup for local development

### 1) Database
First, you need to have a SQL database for local development The database can be hosted locally or a Azure Sql Database. You also need have a valid connection string of the database for the next step.

### 2) Initialize Database
You can find a file named migration.sql under UserManagement.Infrastructure project.

### 3) Configuration
User Management and Identity Service require a local.setting.json with the following configuration under \src\IdentityService and \src\UserManagement respectively.

```
{
  "ConnectionStrings": {
    "Default": "-- Connection String --"
  }
}
```


## Entity Framework Migration

After updating entity of identity, you need run both command to regenerate sql script to update database.

## CREATE DbContext
```
dotnet ef migrations add Initial --startup-project ../UserManagement/
```

## Generate Sql Scripts
```
dotnet ef migrations script --startup-project ../UserManagement/ -i -o migration.sql
```