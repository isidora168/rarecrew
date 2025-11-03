# Employees Assignment (Angular + ASP.NET Core MVC)

This repository contains **two projects** that use the same REST endpoint:  
`https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code={key}`

`{key}` is the Azure Functions **code** (API key) and is passed as `?code=...`

## 1) Angular app

**Folder:** `projectAngular/`  
**Tech:** Angular 18, Chart.js 4, chartjs-plugin-datalabels
### Run
```bash
cd projectAngular
npm install
ng serve

Open in your browser (must include the key in the URL): http://localhost:4200/?code=YOUR_KEY
```
## 2) ASP.NET Core MVC app

**Folder:** `ProjectC/Employees/`  
**Tech:** .NET (ASP.NET Core MVC), SkiaSharp

### Run
```bash
cd ProjectC/Employees/Employees
dotnet build
dotnet run

Open in your browser (must include the key in the URL): https://localhost:<PORT>/Employees?key=YOUR_KEY
