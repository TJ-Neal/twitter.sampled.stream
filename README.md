# Twitter Sampled Stream Project

Sample code for interacting with, tracking, and analysizing data from the [Twitter Sampled Stream](https://developer.twitter.com/en/docs/twitter-api/tweets/volume-streams/quick-start/sampled-stream).

To run the API or Web App an SSL cert is needed. To avoid receiving errors in the browser, the certification should be self-signed and trusted. Using the following command on Windows or Mac should result in a development certificated that is acceptable.

```bash

RUN dotnet dev-certs https -ep path/to/solution/ssl/certs/cert.pfx -p ${USER_DEFINED_PASSWORD(GUID)}$ --trust

```

Currently, `docker-compose.yml` and Kestrel are configured to access the certificate from that specific location in the solution file when the Docker containers are generated.

Alternatively, on Linux based Hosts [openssl](https://learn.microsoft.com/en-us/dotnet/core/additional-tools/self-signed-certificates-guide#with-openssl) or [mkcert](https://github.com/FiloSottile/mkcert) can be used to create a CA root certificate and usable self-signed certificates.