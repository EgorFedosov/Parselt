import { useEffect, useState } from "react";
import { Table, message } from "antd";
import { getLogs } from "../services/api";
import ParsingRowDetail from "./ParsingRowDetail";

export default function LogsTable({ fileId }) {
  const [logs, setLogs] = useState([]);
  const [loading, setLoading] = useState(false);

  const typeMap = {
    ParseCsv: "Парсинг CSV",
    UploadFile: "Загрузка файла",
    SaveToDatabaseRawFile: "Сохранение в БД",
  };

  const statusMap = {
    Success: "Успешно",
    Failed: "Ошибка",
    Partial: "Частично",
  };

  useEffect(() => {
    const fetchLogs = async () => {
      setLoading(true);
      try {
        const res = await getLogs(fileId);
      
        setLogs(res.data.operations || []);
      } catch (e) {
        if (e.response?.status === 404) {
          message.info("Логи не найдены");
        } else {
       
          message.error("Ошибка при получении логов");
        }
      } finally {
        setLoading(false);
      }
    };

    fetchLogs();
  }, [fileId]);

  const columns = [
    { title: "ID", dataIndex: "id", key: "id" },
    {
      title: "Тип операции",
      dataIndex: "type",
      key: "type",
      render: (v) => typeMap[v] || v,
    },
    {
      title: "Статус",
      dataIndex: "status",
      key: "status",
      render: (v) => statusMap[v] || v,
    },
    {
      title: "Начало",
      dataIndex: "startAt",
      key: "startAt",
      render: (text) => new Date(text).toLocaleString("ru-RU"),
    },
    {
      title: "Окончание",
      dataIndex: "finishAt",
      key: "finishAt",
      render: (text) => (text ? new Date(text).toLocaleString("ru-RU") : "-"),
    },
    { title: "Всего строк", dataIndex: "totalRows", key: "totalRows" },
  ];

  return (
    <Table
      columns={columns}
      dataSource={logs}
      rowKey="id"
      loading={loading}
      pagination={{ pageSize: 10 }} 
      expandable={{
        expandedRowRender: (record) =>
          record.type === "ParseCsv" ? (
            <ParsingRowDetail operationId={record.operationId} />
          ) : null,
        rowExpandable: (record) => record.type === "ParseCsv",
      }}
    />
  );
}
