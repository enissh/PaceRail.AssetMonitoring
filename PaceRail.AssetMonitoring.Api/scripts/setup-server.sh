#!/bin/bash
# Linux Server Bootstrap & Nginx Configuration Script for PACE Infrastructure Droplet

set -e

echo "=== 1. Updating System Packages ==="
sudo apt update && sudo apt upgrade -y

echo "=== 2. Installing .NET 10 Runtime, Nginx, UFW, and PostgreSQL ==="
sudo apt install -y dotnet-runtime-10.0 nginx ufw postgresql postgresql-contrib

echo "=== 3. Configuring UFW Firewall ==="
sudo ufw allow OpenSSH
sudo ufw allow 'Nginx Full'
sudo ufw --force enable

echo "=== 4. Setting Up Web Application Directory ==="
sudo mkdir -p /var/www/pacerail-api
sudo chown -R $USER:$USER /var/www/pacerail-api

echo "=== 5. Nginx Reverse Proxy Configuration ==="
cat << 'EOF' | sudo tee /etc/nginx/sites-available/pacerail-api
server {
    listen 80;
    server_name api.pacerail.co.uk;

    location / {
        proxy_pass         http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
EOF

sudo ln -sf /etc/nginx/sites-available/pacerail-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx

echo "=== Setup Complete! ==="