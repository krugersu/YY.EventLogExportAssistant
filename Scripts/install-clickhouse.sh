#!/bin/sh

sudo apt-get install apt-transport-https ca-certificates dirmngr
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv E0C56BD4

echo "deb https://repo.clickhouse.tech/deb/stable/ main/" | sudo tee \
    /etc/apt/sources.list.d/clickhouse.list
sudo apt-get update

sudo apt-get install -y clickhouse-server clickhouse-client

sudo sed -i 's/<password><\/password>/<password>defaultpassword<\/password>/g' /etc/clickhouse-server/users.xml
sudo sed -i 's/<!-- <listen_host>0.0.0.0<\/listen_host> -->/<listen_host>0.0.0.0<\/listen_host>/g' /etc/clickhouse-server/config.xml

sudo cat /etc/clickhouse-server/config.xml
sudo cat /etc/clickhouse-server/users.xml

sudo service clickhouse-server stop
sleep 30

sudo service clickhouse-server start
sleep 30

clickhouse-client --query "SELECT 'Connection OK'" --password defaultpassword
clickhouse-client --query "SELECT version()" --password defaultpassword