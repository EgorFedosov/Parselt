import { Space, Steps } from "antd";
import { useState } from "react";
import { UploadPage, SetupPage, ResultPage, HistoryPage } from "./pages";

function App() {
  const [current, setCurrent] = useState(0);
  const [uploadedFile, setUploadedFile] = useState(null);
  const [fileId, setFileId] = useState(null);
  const [parsedRules, setParsedRules] = useState(null);
  const pages = [
    {
      title: "Загрузка",
      element: UploadPage,
      props: {
        uploadedFile,
        setUploadedFile,
        setFileId,
        onNextPage: () => setCurrent(current + 1),
      },
    },
    {
      title: "Настройка",
      element: SetupPage,
      props: {
        uploadedFile,
        fileId,
        onNextPage: () => setCurrent(current + 1),
        onSaveRules: (rules) => setParsedRules(rules),
      },
    },
    {
      title: "Результат",
      element: ResultPage,

      props: {
        parsedRules,
        uploadedFile,
        fileId,
        onNextPage: () => setCurrent(current + 1),
      },
    },
    {
      title: "История",
      element: HistoryPage,
      props: {},
    },
  ];

  const CurrentPage = pages[current].element;
  const currentPageProps = pages[current].props;

  return (
    <Space
      direction="vertical"
      style={{
        width: "100%",
      }}
    >
      <Steps
        current={current}
        onChange={setCurrent}
        items={pages.map((p) => ({ title: p.title }))}
      ></Steps>
      <CurrentPage {...currentPageProps} />
    </Space>
  );
}

export default App;
