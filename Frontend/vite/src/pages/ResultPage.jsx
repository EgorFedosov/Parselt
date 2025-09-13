import { useEffect, useState } from "react";
import { getLog, getParsedResult, saveResultData } from "../services/api";
import { Card, Typography, Spin, Space, Button, App } from "antd";
import HistoryPanel from "../components/HistoryPanel";
import ResultTable from "../components/ResultTable";

export default function ResultPage({ parsedRules, fileId, onNextPage }) {
  const [loading, setLoading] = useState(true);
  const [parsedResult, setParsedResult] = useState(null);
  const [operationId, setOperationId] = useState(null);
  const [key, setKey] = useState(null);
  const [logData, setLogData] = useState(null);
  const { message } = App.useApp();
  const statusMap = {
    Success: "Успешно",
    Failed: "Ошибка",
    Partial: "Частично",
  };

  useEffect(() => {
    if (!parsedRules) return;

    const fetchParse = async () => {
      setLoading(true);
      try {
        const res = await getParsedResult(parsedRules);

        setKey(res.data.key);
        setParsedResult(res.data.result);
        setOperationId(res.data.result.operationId);
      } catch (e) {
        console.log("Ошибка получения результата:", e);
      } finally {
        setLoading(false);
      }
    };
    fetchParse();
  }, [parsedRules]);

  useEffect(() => {
    if (!operationId) return;

    const fetchLog = async () => {
      setLoading(true);
      try {
        const res = await getLog(operationId);
        setLogData(res.data);
      } catch (e) {
        console.log("Ошибка получения лога:", e);
      } finally {
        setLoading(false);
      }
    };
    fetchLog();
  }, [operationId]);

  const handleSaveResult = async () => {
    try {
      console.log("key:", key);
      console.log("fileId:", fileId);
      console.log("operationId:", operationId);
      await saveResultData(key, fileId, operationId);
      message.success("Результат успешно сохранён в БД");
    } catch (e) {
      console.log("Ошибка при сохранении результата в БД", e);
      message.error("Ошибка при сохранении результата");
    }
  };

  const rows = parsedResult?.rows ?? [];
  const columns = Object.keys(rows[0]?.parsedValues ?? {}).map((col) => ({
    title: col,
    dataIndex: col,
    key: `col_${col}`,
  }));

  const dataSource = rows.map((row) => ({
    ...row.parsedValues,
    rowIndex: row.rowIndex,
    isValid: row.isValid,
  }));

  const errorColumns = [
    {
      title: "Сообщение об ошибке",
      dataIndex: "message",
      key: "message",
    },
  ];

  return (
    <Card title="3) Результат">
      {loading ? (
        <Spin />
      ) : dataSource.length > 0 ? (
        <>
          <ResultTable columns={columns} dataSource={dataSource} />

          <HistoryPanel
            logData={logData}
            statusMap={statusMap}
            errorColumns={errorColumns}
          />
          <Space
            style={{ marginTop: 20, width: "100%", justifyContent: "flex-end" }}
          >
            <Button
              type="primary"
              onClick={handleSaveResult}
              style={{ marginRight: 8 }}
            >
              Сохранить результат в базу данных
            </Button>
            <Button type="primary" onClick={onNextPage}>
              Посмотреть историю за всё время
            </Button>
          </Space>
        </>
      ) : (
        <Typography.Text>Нет данных</Typography.Text>
      )}
    </Card>
  );
}
