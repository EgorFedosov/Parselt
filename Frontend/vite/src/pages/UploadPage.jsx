import { useState } from "react";
import { Card, Col, Row, Tag, Button } from "antd";
import UploadDragger from "../components/UploadDragger";

export default function UploadPage({
  uploadedFile,
  setUploadedFile,
  setFileId,
  onNextPage,
}) {
  const [uploading, setUploading] = useState(false);

  return (
    <Card title="1) Загрузите CSV">
      <UploadDragger
        onUpload={(fileName, fileId) => {
          setUploadedFile(fileName);
          setFileId(fileId);
        }}
        uploading={uploading}
        onUploadStart={() => setUploading(true)}
        onUploadError={() => setUploading(false)}
      />

      <Row justify="end" style={{ marginTop: 24 }}>
        <Col>
          <Button type="primary" onClick={onNextPage} disabled={!uploadedFile}>
            Далее
          </Button>
        </Col>
      </Row>
    </Card>
  );
}
