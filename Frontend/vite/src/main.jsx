import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.jsx";
import { App as AntdApp } from "antd";

createRoot(document.getElementById("root")).render(
  <AntdApp>
    <StrictMode>
      <App />
    </StrictMode>
  </AntdApp>
);
