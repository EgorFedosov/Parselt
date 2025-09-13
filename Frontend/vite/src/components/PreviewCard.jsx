import { Card, Typography, Spin, Space, Table } from "antd";

export default function PreviewCard({ loading, dataSource, columns }) {
  return (
    <Card title="2) Настройка">
      {loading ? (
        <div
          style={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          <Space size="middle">
            <Typography.Text>Загружается...</Typography.Text>
            <Spin />
          </Space>
        </div>
      ) : dataSource.length > 0 ? (
        <Table
          columns={columns}
          dataSource={dataSource}
          pagination={false}
          bordered
          size="small"
        />
      ) : (
        <Typography.Paragraph>Нет данных</Typography.Paragraph>
      )}
    </Card>
  );
}
