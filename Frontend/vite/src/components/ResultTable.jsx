import { Table } from "antd";

export default function ResultTable({columns,dataSource}){
    return (
      <Table
        rowKey="rowIndex"
        columns={columns}
        dataSource={dataSource}
        pagination={{ pageSize: 10 }}
      />
    );
}