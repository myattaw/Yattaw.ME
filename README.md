# Yattaw.ME

A **fully serverless personal website** for [Yattaw.ME](https://Yattaw.ME), showcasing personal experience and public GitHub repositories.

This project leverages **React** for a dynamic frontend and **C#** for serverless backend logic, powered by **AWS Lambda**.

---

## ✨ Features

- 📚 Displays all public GitHub repositories, sorted by commit activity.
- 💼 Showcases personal experience and portfolio details.
- ☁️ **Fully serverless architecture** for high scalability and low maintenance.
- 🔄 Automated GitHub data updates every 30 minutes via AWS Lambda.

---

## 🏗️ Architecture

### 🌐 Frontend
- **React Single-Page Application (SPA)** hosted on ![AWS S3](https://a.b.cdn.console.awsstatic.com/a/v1/DKY2SIL5N3MJQCULDNOQE7TKLNQIUXRSOHBJKJGQAHLZO7TLH3TQ/icon/c0828e0381730befd1f7a025057c74fb-43acc0496e64afba82dbc9ab774dc622.svg) AWS S3.
- Fetches data from the backend via ![AWS API Gateway](https://a.b.cdn.console.awsstatic.com/a/v1/YQSXE26XPXPOFR4RNTHADZ6A5EBPBODPAKV6IERNZE66HMBAER2A/icon/fb0cde6228b21d89ec222b45efec54e7-0856e92285f4e7ed254b2588d1fe1829.svg) API Gateway (served by AWS Lambda).
- Displays GitHub repositories (commits, stars) and personal experience.

### ⚙️ Backend
1. **GitHubFetcher.Yattaw.ME (C# Class Library)**  
   - Runs as an ![AWS Lambda](https://a.b.cdn.console.awsstatic.com/a/v1/BMZQS7MWY7VIUF7PXETK3ULHIXZQQOURXD3AK46KD7UE6WMRLUSA/icon/945f3fc449518a73b9f5f32868db466c-926961f91b072604c42b7f39ce2eaf1c.svg) AWS Lambda function.
   - Triggered every 30 minutes by ![AWS EventBridge](https://a.b.cdn.console.awsstatic.com/a/v1/VE337T2LHHXFIAPFSE6PV7E2KJFUWYNWTAIFJYCSMCOOD6VG7ZHQ/icon/16908b0605f2645dfcb4c3a8d248cef3-8fdd092f1116685eeb75b950acb85987.svg) EventBridge.
   - Fetches GitHub repository data and updates ![AWS DynamoDB](https://a.b.cdn.console.awsstatic.com/a/v1/AN2R6BU3DBLYCROPWJWYQWM62AYYLMXTM5V7AHNGQIU34L2VIEEA/icon/6f419a45e63123b4c16bd679549610f6-87862c68693445999110bbd6a467ce88.svg) DynamoDB.

2. **Data API (C# ASP.NET Lambda)**  
   - Serves data from DynamoDB to the React frontend.
   - Handles repository and experience data for display.

---

## 🛠️ Tech Stack

| Component       | Technologies                                                                 |
|-----------------|------------------------------------------------------------------------------|
| **Frontend**    | React, TailwindCSS          |
| **Backend**     | C# .NET, ASP.NET Core, ![AWS Lambda](https://a.b.cdn.console.awsstatic.com/a/v1/BMZQS7MWY7VIUF7PXETK3ULHIXZQQOURXD3AK46KD7UE6WMRLUSA/icon/945f3fc449518a73b9f5f32868db466c-926961f91b072604c42b7f39ce2eaf1c.svg) AWS Lambda |
| **Storage**     | ![AWS DynamoDB](https://a.b.cdn.console.awsstatic.com/a/v1/AN2R6BU3DBLYCROPWJWYQWM62AYYLMXTM5V7AHNGQIU34L2VIEEA/icon/6f419a45e63123b4c16bd679549610f6-87862c68693445999110bbd6a467ce88.svg) AWS DynamoDB |
| **Hosting**     | ![AWS S3](https://a.b.cdn.console.awsstatic.com/a/v1/DKY2SIL5N3MJQCULDNOQE7TKLNQIUXRSOHBJKJGQAHLZO7TLH3TQ/icon/c0828e0381730befd1f7a025057c74fb-43acc0496e64afba82dbc9ab774dc622.svg) AWS S3 (Frontend), ![AWS Lambda](https://a.b.cdn.console.awsstatic.com/a/v1/BMZQS7MWY7VIUF7PXETK3ULHIXZQQOURXD3AK46KD7UE6WMRLUSA/icon/945f3fc449518a73b9f5f32868db466c-926961f91b072604c42b7f39ce2eaf1c.svg) AWS Lambda (Backend) |
| **Automation**  | ![AWS EventBridge](https://a.b.cdn.console.awsstatic.com/a/v1/VE337T2LHHXFIAPFSE6PV7E2KJFUWYNWTAIFJYCSMCOOD6VG7ZHQ/icon/16908b0605f2645dfcb4c3a8d248cef3-8fdd092f1116685eeb75b950acb85987.svg) AWS EventBridge |
