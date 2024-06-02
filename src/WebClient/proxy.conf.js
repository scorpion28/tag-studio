// https://learn.microsoft.com/en-us/dotnet/aspire/get-started/build-aspire-apps-with-nodejs#explore-the-angular-client

module.exports = {
  "/api": {
    target:
      process.env["services__webapi__https__0"] ||
      process.env["services__webapi__http__0"],
    secure: process.env["NODE_ENV"] !== "development",
    pathRewrite: {
      "^/api": "",
    },
  },
};
