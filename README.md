![Hackathon Logo](docs/images/Teams-Notification-Architect.png?raw=true "Teams Notification Architect")

# Sitecore Hackathon 2025

## Team name
⟹ SRS

## Category
⟹ Integration

## Description
⟹ Sitecore XMCloud to Microsoft Teams Notification Workflow  

  - This entry is the workflow for notifying a Microsoft Teams channel whenever a new item is added in Sitecore XMCloud. The integration involves Sitecore Webhooks, an Azure Function, and a Teams channel workflow.

## Video link
⟹ 

## Pre-requisites and Dependencies

⟹ Teams channel workflow 

⟹ Azure Function in Visual Studio

⟹ Deployment of Azure Function in Azure

⟹ Configure a Webhook in Sitecore


## Installation instructions

> - Create a Microsoft Teams Channel and Configure a Workflow
> - Develop an Azure Function in Visual Studio and specify Teams webhook
> - Configure a Webhook in Sitecore
> - Data Flow and Execution


### Configuration
⟹ Develop an Azure Function in Visual Studio and specify Teams webhook in line 18 in Function1.cs as

private static readonly string webhookUrl = "<MS Teams webhook Url>";


## Usage instructions

1. Login to Sitecore.
2. Click on Content Editor.
3. Create Webhook Event Handler at “/sitecore/system/Webhooks/Azure Team Webhook” and specify Azure Function endpoint in URL field and Enabled it
4. Create an item in Sitecore Content Tree.
5. Check Teams notification for this item creation.
