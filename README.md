
# User-Service
The user service is responsible for handling the organization information and users within the organization information. 

An Organization should be added to the database connected to the userservice 
which then users can be added to said organization 
Users:
- Patients
- Caregivers 

## Important Note
Caregivers can only be added via azure and not through the userservice and changes to the caregivers have to be made in azure.

The user role caregiver is gotten from azure using auzre graph and then added to the userservice postgresql database

After users have been created, the userservice can then be used to add those users in to patientgroups. Which will be used and displayed in the caregiver dashboard.

## Running the service alone
* docker compose up 
Important Note: 
- When running the service alone, you would have to update the database information (User and Password) on the userservice appsettings.
- Update the docker compose port for the user service to 5054

## Ports: 5054 & 8086
* The User Service runs on port **5054**.
* The corresponding database runs on port **5432**.

## Database
The User service makes use of PostgreSQL database 
- Recommendation to download a local postgresql when running the service alone, to track database information.

## API endpoints
```
All Api endpoints and a brief description of each, can be found in the Software Architectural document.
```

## Nats
```
Code for Nats in this project is currently commented out, therefore it is recommended to either have a look at it, if the current developer has previous knowledge on Nats or If the current developer does not then start from scratch regarding the message broker.
```

## Docker
If you want to manually build a Docker container of this service and push, use the following commands in a CLI:
```
docker build -t ghcr.io/fontys-stress-wearables/user-service:main .
```
Then
```
docker push ghcr.io/fontys-stress-wearables/user-service:main
```
