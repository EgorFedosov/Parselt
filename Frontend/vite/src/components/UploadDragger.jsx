import { Upload, message } from "antd";
import { InboxOutlined } from "@ant-design/icons";
import { uploadFiles } from "../services/api.js";

const { Dragger } = Upload;

export default function UploadDragger({
  onUpload,
  uploading,
  onUploadStart,
  onUploadError,
}) {
  const customRequest = async ({ file, onSuccess, onError }) => {
    onUploadStart?.();
    try {
      const formdata = new FormData();
      formdata.append("file", file);
      const res = await uploadFiles(formdata);
      onUpload?.(res.data.metaData[0].fileName, res.data.metaData[0].id);
      onSuccess?.(res.data);
    } catch (e) {
      onError?.(e);
      onUploadError?.();
      message.error("Ошибка загрузки файла");
    }
  };

  return (
    <Dragger
      name="file"
      customRequest={customRequest}
      disabled={uploading}
      accept=".csv"
    >
      <p className="ant-upload-drag-icon">
        <InboxOutlined />
      </p>
      <p className="ant-upload-text">Перетащите файл или кликните для выбора</p>
      <p className="ant-upload-hint">Поддерживаются только CSV</p>
    </Dragger>
  );
}
