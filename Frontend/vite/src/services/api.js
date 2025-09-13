import axios, { Axios } from "axios";
import { saveAs } from "file-saver";

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 7000,
});

export const uploadFiles = async (formData) => {
  try {
    const res = await apiClient.post("/upload", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    return res;
  } catch (e) {
    console.error("Ошибка загрузки файла:", e);
    throw e;
  }
};
export const getPreview = async (fileName) => {
  try {
    const data = await apiClient.get(`/preview/${fileName}`);
    return data;
  } catch (e) {
    console.log("Ошибка предпросмотра:", e);
    throw e;
  }
};
export const getParsedResult = async (rules) => {
  try {
    const data = await apiClient.post("/parse", rules);
    console.log("Получаем такие данные", data)
    return data;
  } catch (e) {
    console.log("Ошибка парсинга:", e);
    throw e;
  }
};

export const getLog = async (operationId) => {
  try {
    const data = await apiClient.get(`/log/${operationId}`);
    return data;
  } catch (e) {
    console.log("Ошибка получения лога:", e);
    throw e;
  }
};

export const saveResultData = async (key, fileId, operationId) => {
  try {
    const data = await apiClient.post(
      `/parse/save/result/${key}/${fileId}/${operationId}`
    );
    return data;
  } catch (e) {
    console.log("Ошибка сохранения результата:", e);
    throw e;
  }
};
export const getFiles = async () => {
  try {
    const data = await apiClient.get(`files`);
    return data;
  } catch (e) {
    console.log("Ошибка получения списка файлов из БД:", e);
    throw e;
  }
};

export const getFilesByName = async (fileName) => {
  try {
    const data = await apiClient.get(`files`, { params: { fileName } });
    return data;
  } catch (e) {
    console.error(e);
    throw e;
  }
};

export const getRawFile = async (fileId) => {
  try {
    const data = await apiClient.get(`files/${fileId}/raw`);
    return data;
  } catch (e) {
    console.error(e);
    throw e;
  }
};

export const getResult = async (operationId) => {
  try {
    const data = await apiClient.get(`files/${operationId}/results`);
    return data;
  } catch (e) {
    console.log(
      "Произошла ошибка при получении результата из БД, operation id:",
      operationId
    );
    console.error(e);
    throw e;
  }
};

export const downloadRow = async (fileId) => {
  try {
    const response = await apiClient.get(`/files/${fileId}/download-row`, {
      responseType: "blob",
    });

    const disposition = response.headers["content-disposition"];
    const fileName = disposition
      ? disposition.split("filename=")[1].split(";")[0].replace(/"/g, "")
      : "file.csv";

    saveAs(response.data, fileName);
  } catch (e) {
    console.error("Ошибка при скачивании:", e);
  }
};

export const getLogs = async (fileId) => {
  try {
    const data = await apiClient.get(`files/${fileId}/logs`);
    return data;
  } catch (e) {
    console.error(e);
    throw e;
  }
};

export const getLogsErrors = async (operationId) => {
  try {
    const data = await apiClient.get(`files/${operationId}/logs/errors`);
    return data;
  } catch (e) {
    console.error(e);
    throw e;
  }
};

export const downloadResult = async (operationId) => {
  try {
    const response = await apiClient.get(
      `files/${operationId}/download-result`,
      {
        responseType: "blob",
      }
    );
    console.log(response.headers);
    const disposition =
      response.headers["content-disposition"] ||
      response.headers["Content-Disposition"];

    const fileName = disposition
      ? disposition.split("filename=")[1].split(";")[0].replace(/"/g, "")
      : "file.csv";

    saveAs(response.data, fileName);
  } catch (e) {
    console.error("Ошибка при скачивании:", e);
    throw e;
  }
};
