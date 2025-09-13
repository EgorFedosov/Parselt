import { useEffect, useState } from "react";
import { Tabs, Table, App, Button } from "antd";
import { getLogsErrors, getResult, downloadResult } from "../services/api";
import ResultTable from "./ResultTable";
import ErrorsTable from "./ErrorsTable";

export default function ParsingRowDetail({ operationId }) {
  const [resultRows, setResultRows] = useState([]);
  const [errors, setErrors] = useState([]);
  const [loadingErrors, setLoadingErrors] = useState(false);
  const { message } = App.useApp();

  useEffect(() => {
    const fetchResult = async () => {
      try {
        const res = await getResult(operationId);
        setResultRows(res.data || []);
        console.log("result: ", res);
      } catch (e) {
        if (e.response?.status === 404) {
          message.info("Результаты не найдены");
        } else {
          console.error("Ошибка при получении результатов:", e);
          message.error("Ошибка при получении результатов");
        }
      }
    };

    const fetchErrors = async () => {
      setLoadingErrors(true);
      try {
        const res = await getLogsErrors(operationId);
        setErrors(res.data.errors || []);
      } catch (e) {
        if (e.response?.status === 404) {
          message.info("Ошибки не найдены");
        } else {
          console.error("Ошибка при получении ошибок:", e);
          message.error("Ошибка при получении ошибок");
        }
      } finally {
        setLoadingErrors(false);
      }
    };

    fetchResult();
    fetchErrors();
  }, [operationId]);

  const columns = Object.keys(resultRows[0]?.parsedValues ?? {}).map((col) => ({
    title: col,
    dataIndex: col,
    key: `col_${col}`,
  }));

  const dataSource = resultRows.map((row) => ({
    ...row.parsedValues,
    rowIndex: row.rowIndex,
    isValid: row.isValid,
  }));

  const errorColumns = [
    { title: "Сообщение", dataIndex: "message", key: "message" },
  ];

  return (
    <>
      <Tabs
        defaultActiveKey="1"
        items={[
          {
            key: "1",
            label: "Результат",
            children: (
              <>
                <ResultTable columns={columns} dataSource={dataSource} />
                <Button
                  type="primary"
                  onClick={() => downloadResult(operationId)}
                >
                  Скачать
                </Button>
              </>
            ),
          },
          {
            key: "2",
            label: "Ошибки",
            children: (
              <ErrorsTable
                errorColumns={errorColumns}
                errors={errors}
                loadingErrors={loadingErrors}
              />
            ),
          },
        ]}
      />
    </>
  );
}
