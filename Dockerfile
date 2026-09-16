FROM nginx:alpine
COPY ./PaceRail.AssetMonitoring.Api/wwwroot /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]