module.exports = {
  "/api": {
    target:
      process.env["services__appointmentspanel-server__https__0"] ||
      process.env["services__appointmentspanel-server__http__0"],
    secure: process.env["NODE_ENV"] !== "development",
    pathRewrite: {
      "^/api": "",
    },
  },
  "/api/hub": {
    target:
      process.env["services__appointmentspanel-server__https__0"] ||
      process.env["services__appointmentspanel-server__http__0"],
    ws: true,
    secure: process.env["NODE_ENV"] !== "development",
    pathRewrite: {
      "^/api/hub": "hub",
    },
  },
};
