import { Table } from "antd";

export default function ErrorsTable({ errorColumns, errors, loadingErrors }) {
  return (
    <Table
      columns={errorColumns}
      dataSource={errors.map((e, idx) => ({ key: idx, ...e }))}
      loading={loadingErrors}
      pagination={{ pageSize: 10 }}
    />
  );
}
