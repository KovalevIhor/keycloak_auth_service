** RUN

docker-compose up -d keycloak

`Username`: admin
`Password`: admin

** Project configuration

Configure `appsettings.json`

```json
{
 "Keycloak": {
  "realm": "master",
  "auth-server-url": "http://localhost:8080/",
  "ssl-required": "none",
  "resource": "<CLIENT_ID>",
  "verify-token-audience": false,
  "credentials": {
   "secret": "<CLIENT_SECRET>"
  },
  "confidential-port": 0
 }
}
```


** Create App client

1) Clients -> Create client
2) Put client id and name 
![imgs/img.png](img.png)
3) Chose `Client authentication` and `Service accounts roles`
![imgs/img_1.png](img_1.png)
4) Service accounts roles -> Assign role -> Select all roles
![imgs/img_2.png](img_2.png)


** Create Group

1) Groups -> Create group
2) Put group name (ex. `naps_admin`)


** Add user scopes

1) Client scopes -> Create client scopes
![imgs/img_3.png](img_3.png)
2) Mappers -> Configure a new mapper -> User Attribute
![imgs/img_4.png](img.png)


** Generate user auth token

`curl --location --request POST 'http://localhost:8080/realms/master/protocol/openid-connect/token' \
 --header 'Content-Type: application/x-www-form-urlencoded' \
 --data-urlencode 'client_id=<CLIENT_ID>' \
 --data-urlencode 'client_secret=<CLIENT_SECRET>' \
 --data-urlencode 'grant_type=password' \
 --data-urlencode 'username=<USER_USERNAME>' \
 --data-urlencode 'password=<USER_PASSWORD>' \
 --data-urlencode 'scope=email profile fleet_id'`