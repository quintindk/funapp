# Funapp - An Azure Function App template Pipeline

[![Infrastructure](https://dev.azure.com/quintindk/FunApp/_apis/build/status%2Fquintindk.funapp?branchName=refs%2Fpull%2F5%2Fmerge)](https://dev.azure.com/quintindk/FunApp/_build/latest?definitionId=1&branchName=refs%2Fpull%2F5%2Fmerge)
[![Code](https://dev.azure.com/quintindk/FunApp/_apis/build/status%2Fquintindk.funapp%20(1)?branchName=refs%2Fpull%2F5%2Fmerge)](https://dev.azure.com/quintindk/FunApp/_build/latest?definitionId=2&branchName=refs%2Fpull%2F5%2Fmerge)

This repository contains the base files and instructions to create a funapp i.e An Azure Function App in different languages.

## Getting Started

Firstly install the function app core tools. This is a summary of the core tools page [here](https://github.com/Azure/azure-functions-core-tools/blob/v4.x/README.md)

### Linux/WSL

```bash
wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install azure-functions-core-tools-4
```

### Windows

```powershell
winget install Microsoft.AzureFunctionsCoreTools
```

## Create your Project

To to create the project you can just follow the guide [here](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=azure-cli%2Cin-process).

```powershell
func init LocalFunctionProj --dotnet
cd LocalFunctionProj
```

Then you can simply Add a function to your project by using the following command, where the --name argument is the unique name of your function and the --template argument specifies the function's trigger (HTTP).

```powershell
func new --name TestHttpExample --template "HTTP trigger" --authlevel "anonymous"
```

## Run your Function locally

Run your function by starting the local Azure Functions runtime host from the LocalFunctionProj folder:

```powershell
func start
```
