# escape=`

ARG BASE_IMAGE
ARG BUILD_IMAGE
FROM ${BUILD_IMAGE} as build

COPY docs/Reference.md ./docs/

WORKDIR /src/Middleware

COPY src/Middleware/ .

RUN dotnet restore src/Headstart.API/Headstart.API.csproj

RUN dotnet build src/Headstart.API/Headstart.API.csproj --configuration Release

RUN dotnet publish src/Headstart.API/Headstart.API.csproj --configuration Release --output /out/middleware

FROM ${BASE_IMAGE} as production

RUN apk add --update nodejs nodejs-npm

RUN npm install -g json

RUN apk add --no-cache --upgrade bash

WORKDIR /app

COPY --from=build /out/middleware .

COPY docs/Reference.md ./reference.md

COPY docker/build/middleware/* ./

RUN apk add dos2unix && dos2unix /app/entrypoint.sh

EXPOSE 80

RUN ["chmod", "+x", "/app/entrypoint.sh"]

CMD ["sh", "/app/entrypoint.sh"]
