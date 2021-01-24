# Run the application in a docker container

If you want to, you can run the API in a docker container. The steps below explain how to build the image for the API, run the container and then view the logs in the container. The below steps expect you have docker already installed. The steps were tested on a Windows 10 machine with Docker Desktop v3.1.0 and Docker Engine v20.10.2 with WSL2 as the underlying engine.

1. Clone the repo and then navigate to the repo's root folder in a shell with the docker daemon running.
2. Run the below command to build the image:

```
docker build . -f ./Deploy/Dockerfile -t checkoutpaymentapi
```

3. Once that is complete, run the below command to run a new container called `checkoutpaymentapiapp` with the image, the command connects port 8080 on your system with port 80 in the container

```
docker run -p 8080:80 --name checkoutpaymentapiapp checkoutpaymentapi
```

4. Navigate to `http://localhost:8080/swagger` in a browser to view the end point docs for the API. Or hit an endpoint in Postman.

5. Finally, view the logs of the API with the below command. Replace the date format with today's date. 

```
docker exec checkoutpaymentapiapp cat log-YYYYMMDD.log
```

If you aren't able to find the file, ensure you've made at least one request to the API and try again
