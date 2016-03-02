FROM microsoft/aspnet:1.0.0-rc1-final-coreclr

WORKDIR /app
EXPOSE 5000
ENTRYPOINT ["dnx", "web"]

COPY project.json .
RUN dnu restore

ADD . ./
