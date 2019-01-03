FROM mono:latest
RUN mkdir /app
RUN mkdir /app/logs
COPY . /app
EXPOSE 3579
WORKDIR /app
CMD ["mono", "/app/NerdBot.exe"]