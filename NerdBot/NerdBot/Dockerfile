FROM mono:latest
RUN mkdir /app
COPY . /app
EXPOSE 3579
WORKDIR /app
CMD ["mono", "/app/NerdBot.exe"]