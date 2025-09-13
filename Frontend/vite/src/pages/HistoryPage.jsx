import { useEffect, useState } from "react";
import {
  getFiles,
  getFilesByName,
  getRawFile,
  downloadRow,
} from "../services/api";
import LogsTable from "../components/LogsTable";
import { Table, Card, Button, App, Tabs } from "antd";
import FileSearch from "../components/FileSearch";

export default function HistoryPage() {
  const [files, setFiles] = useState([]);
  const [fileNameForSearch, setFileNameForSearch] = useState("");
  const [loading, setLoading] = useState(false);
  const [expandedRows, setExpandedRows] = useState([]);
  const { message } = App.useApp();
  const rawColumns = [{ title: ``, dataIndex: "rawValue", key: "rawValue" }];

  const columns = [
    { title: "ID", dataIndex: "id", key: "id" },
    { title: "Имя файла", dataIndex: "fileName", key: "fileName" },
    { title: "Размер (байт)", dataIndex: "size", key: "size" },
    {
      title: "Дата загрузки",
      dataIndex: "uploadedAt",
      key: "uploadedAt",
      render: (text) => new Date(text).toLocaleString("ru-RU"),
    },
    {
      title: "Детали",
      key: "details",
      render: (_, record) => (
        <Button onClick={() => handleDetailsClick(record)}>Подробнее</Button>
      ),
    },
  ];

  const expandedRowRender = (record) => {
    return (
      <Tabs
        defaultActiveKey="1"
        items={[
          {
            key: "1",
            label: "Исходные данные",
            children: (
              <>
                <Table
                  columns={rawColumns}
                  dataSource={
                    Array.isArray(record.rawFiles) ? record.rawFiles : []
                  }
                  pagination={{ pageSize: 10 }}
                />
                <Button
                  type="primary"
                  onClick={() => handleDownloadRow(record.id)}
                >
                  Скачать
                </Button>
              </>
            ),
          },
          {
            key: "2",
            label: "Логи",
            children: <LogsTable fileId={record.id} />,
          },
        ]}
      />
    );
  };

  const handleDownloadRow = async (fileId) => {
    try {
      await downloadRow(fileId);
    } catch (e) {
      console.error("Ошибка при скачивании строки:", e);
      message.error("Не удалось скачать файл");
    }
  };

  const fetchFiles = async (name = "") => {
    setLoading(true);
    try {
      const res = name ? await getFilesByName(name) : await getFiles();
      setFiles(res.data);
      if (name && res.data.length === 0) {
        message.info("Файлы не найдены");
      }
    } catch (e) {
      if (e.response?.status === 404) {
        setFiles([]);
        message.info("Файлы не найдены");
      } else {
        console.error("Ошибка получения файлов:", e);
        message.error("Ошибка при загрузке файлов");
      }
    } finally {
      setLoading(false);
    }
  };

  const fetchRawFiles = async (fileId) => {
    setLoading(true);
    try {
      const res = await getRawFile(fileId);
      const formattedData = (res.data.rawRows || []).map((value, index) => ({
        key: `${fileId}-${index}`,
        rawValue: value,
      }));

      setFiles((prev) =>
        prev.map((f) =>
          f.id === fileId ? { ...f, rawFiles: formattedData } : f
        )
      );

      if (formattedData.length === 0) {
        message.error("Файл не найден");
      }
    } catch (e) {
      if (e.response?.status === 404) {
        message.info("Файл не найден");
      } else {
        console.error("Ошибка получения исходного файла:", e);
        message.error("Ошибка при получении исходного файла");
      }
    } finally {
      setLoading(false);
    }
  };

  const handleDetailsClick = async (record) => {
    if (!record.rawFiles) {
      await fetchRawFiles(record.id);
    }

    setExpandedRows((prev) =>
      prev.includes(record.id)
        ? prev.filter((id) => id !== record.id)
        : [...prev, record.id]
    );
  };

  useEffect(() => {
    fetchFiles();
  }, []);

  return (
    <Card title="История загрузок">
      <FileSearch
        value={fileNameForSearch}
        onChange={(e) => setFileNameForSearch(e.target.value)}
        onSearch={() => fetchFiles(fileNameForSearch)}
      />
      <Table
        rowKey="id"
        columns={columns}
        dataSource={files}
        loading={loading}
        pagination={{ pageSize: 7 }}
        style={{ marginTop: 16 }}
        expandable={{
          expandedRowRender,
          expandedRowKeys: expandedRows,
          onExpand: (_, record) => handleDetailsClick(record),
        }}
      />
    </Card>
  );
}
