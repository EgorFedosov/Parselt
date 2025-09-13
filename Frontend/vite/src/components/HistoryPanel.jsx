import { Collapse, Descriptions, Typography, Table, Spin } from "antd";
export default function HistoryPanel({ logData, statusMap, errorColumns }) {
  return (
    <Collapse style={{ marginTop: 16 }}>
      <Collapse.Panel header="Просмотреть историю разбора" key="1">
        {logData ? (
          <>
            <Descriptions bordered column={1} size="small">
              <Descriptions.Item label="ID">{logData.id}</Descriptions.Item>
              <Descriptions.Item label="Статус">
                {statusMap[logData.status] ?? logData.status}
              </Descriptions.Item>

              <Descriptions.Item label="Тип">{logData.type}</Descriptions.Item>
              <Descriptions.Item label="Всего строк">
                {logData.totalRows}
              </Descriptions.Item>
              <Descriptions.Item label="Начало">
                {logData.startedAt}
              </Descriptions.Item>
              <Descriptions.Item label="Завершение">
                {logData.finishedAt}
              </Descriptions.Item>
            </Descriptions>

            <Typography.Title level={5} style={{ marginTop: 16 }}>
              Ошибки
            </Typography.Title>
            <Table
              rowKey={(row, idx) => idx}
              columns={errorColumns}
              dataSource={logData.errors ?? []}
              pagination={{ pageSize: 10 }}
            />
          </>
        ) : (
          <>
            <Spin style={{ marginRight: 8 }} />
            <Typography.Text>Нет информации</Typography.Text>
          </>
        )}
      </Collapse.Panel>
    </Collapse>
  );
}
