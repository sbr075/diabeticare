## Server for Diabeticare 
This is an test server for the application "Diabeticare" programmed using Python and Flask 
The server offers a RESTful API without any frontend
All programming and testing was done using a virtual enviroment (using GIT BASH for run.sh script)
**NB!** While the server should run on Linux it has mainly been tested using Windows

## Table of Contents
- [Setting up a Python venv](#setting-up-a-postgresql-database)
  - [Dependencies Installation](#dependencies-installation)
    - [Current Dependencies](#current-dependencies)
- [Setting up a PostgreSQL Database](#setting-up-a-postgresql-database)
  - [Using pgAdmin 4](#using-pgadmin-4)
- [How to Migrate Database Changes](#how-to-migrate-database-changes)
- [Program Usage](#program-usage)
  - [Tests](#tests)
    - [Registration, login, and logout](#registration-login-and-logout)
    - [All Tests](#all-tests)
- [Important Changes](#important-changes)

## How to set up a venv
1. Create the virtual enviroment
> python3 -m venv venv
2. Activate the virtual enviroment
> . venv/Scripts/activate

### Dependencies Installation
To install all dependencies navigate to the main folder and run the command
> python3 -m pip install -r requirements.txt

#### Current Dependencies
A file has been created called [requirements](requirements.txt) for easy install of all required dependencies  

### Setting up a PostgreSQL Database
It is recommended to have a database mangner, e.g., pgAdmin 4, to set up and manage the database
  
#### Using pgAdmin 4
Using pgAdmin to manage postgresql server 
1. Download the executable from https://www.enterprisedb.com/downloads/postgres-postgresql-downloads
2. During installation set the postgres password
**NB!** This password needs to match the database password found in the [config](config.py) file
**NB!** It is recommended to change the password to something only known to you

## How to Migrate Database Changes
Before starting up the server for the first time a migration needs to be ran
This pushes the database schema onto the PostgreSQL database  
To migrate the database do the following
1. Navigate to the 'src' folder
2. Run 'sh diabeticare/migrate.sh' in your terminal
**NB!** On Windows an special terminal is required, e.g., Git Bash, to run bash scripts

## Program Usage
After the virtual environment has been set up, dependencies installed, and the database has been migrated you are ready to start the server.
To do so navigate to the diabeticare folder and run the following command
> flask run --port XXXX
**NB!** The port can be any valid port, but the application is set to listen to 5000

### Tests
Several tests has been written to test the different components of the server
To run tests make sure the Flask server is running and navigate to the [test](test/) folder.

#### Registration, login, and logout
To use run any of the tests an active account is required. To register an account run the following test
> python3 test.py -T 1
*You will be prompted to enter a username and password*

After an account has been registered, you need to run test 2 before any other test
This is needed because the server requires a valid CSRF Token to authenticate requests
An example test would be
> python3 test.py -T 2 4
*This will login the account and run the add bgl entry test*

#### All Tests
There are currently five components available each with four tests
1. blood glucose levels
  - Test IDs: 4, 5, 6, 7
2. sleep
  - Test IDs: 8, 9, 10, 11
3. carbohydrate intake
  - Test IDs: 12, 13, 14, 15
4. mood
  - Test IDs: 16, 17, 18, 19
5. exercise
  - Test IDs: 20, 21, 22, 23

Tests are in the following order
add - update - get - delete

## Important Changes
There are two highly recommend changes to be done before running the server
1. Changing the SECRET_KEY to a unique, unknown, and long sequence of characters
2. Changing the WTF_CSRF_SECRET_KEY to a unique, unknown, and long sequence of characters
*Ideally these two keys should not match*
  
These keys are used by the server to securely sign requests and CSRF tokens for added security.