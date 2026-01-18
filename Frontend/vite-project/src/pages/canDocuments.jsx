import { useState } from 'react';
import { Card } from '../components/common/Card';
import { FileText, Upload, Eye, CheckCircle, Clock, Edit } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { Modal } from '../components/common/modal';
import { Input } from '../components/common/input';
import { Button } from '../components/common/button';
import { showToast } from '../components/common/toast';

export function CandidateDocuments() {
  const [documents, setDocuments] = useState([]);
  const [actionPopup, setActionPopup] = useState(null); // 'upload' | 
  const [selectedDoc, setSelectedDoc] = useState(null);
  const [file, setFile] = useState(null);

  const { user } = useAuth();


  const fetchDoc = async () => {
    try {

      const ret = await fetch(import.meta.env.VITE_API_URI + '/document-verification/candidate/' + user.candidateId, {
        headers: {
          'Content-Type': 'application/json',
          // 'Authorization': `Bearer ${user.jwtToken}`
        }
      });
      const res = await ret.json();
      if (!ret.ok)
        throw new Error(res.message);
      setDocuments(res);
    } catch (e) {
      console.log(e)
    }
  }

  const handleUpload = async (e) => {
    e.preventDefault();

    if (!file) {
      showToast.error('Missing data', 'Please select document type and file');
      return;
    }

    const formData = new FormData();
    formData.append('ApplicationId', selectedDoc.applicationId);
    formData.append('DocTypeId', selectedDoc.docTypeId);
    formData.append('File', file);

    try {

      const res = await fetch(
        `${import.meta.env.VITE_API_URI}/document-verification/upload`,
        {
          method: 'POST',
          body: formData
        }
      );

      if (!res.ok) throw new Error('Upload failed');

      showToast.success('Uploaded', 'Document uploaded successfully');
    } catch (err) {
      showToast.error('Upload Failed', err.message);
    }
  };

  useState(() => {
    fetchDoc();
  }, [])
  const getStatusBadge = (status) =>
    status === 'Uploaded' ? (
      <span className="inline-flex items-center space-x-1 px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-700 border border-green-200">
        <CheckCircle className="w-3 h-3" />
        <span>Uploaded</span>
      </span>
    ) : (
      <span className="inline-flex items-center space-x-1 px-2 py-1 rounded-full text-xs font-medium bg-yellow-100 text-yellow-700 border border-yellow-200">
        <Clock className="w-3 h-3" />
        <span>Pending</span>
      </span>
    );

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-800">
          Document Verification
        </h1>
        <p className="text-gray-600 mt-1">
          Please upload the required documents within 10 days
        </p>
      </div>

      <Card>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gradient-to-r from-green-50 to-green-100">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">
                  Document Type
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">
                  File
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">
                  Uploaded At
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">
                  Status
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">
                  Action
                </th>
              </tr>
            </thead>

            <tbody className="divide-y divide-gray-200">
              {documents.map((doc) => (
                <tr key={doc.documentId} className="hover:bg-gray-50">
                  <td className="px-6 py-4 font-medium text-gray-800">
                    {doc.documentType.docTypeName}
                  </td>

                  <td className="px-6 py-4 text-gray-600 text-sm">
                    {doc.documentUrl || '-'}
                  </td>

                  <td className="px-6 py-4 text-gray-600 text-sm">
                    {doc.uploadedAt ? new Date(doc.uploadedAt).toLocaleDateString('en-IN', {
                      hour: 'numeric',
                      minute: '2-digit',
                      hour12: true
                    }) : '-'}
                  </td>

                  <td className="px-6 py-4">
                    {getStatusBadge(doc.uploadedAt != null ? (doc.isVerified ? 'Verified' : 'Uploaded') : 'Pending')}
                  </td>

                  <td className="px-6 py-4">
                    {doc.uploadedAt != null ? (
                      <div className="flex items-center space-x-2">
                        <button className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg"
                          onClick={() => {
                            setSelectedDoc(doc);
                            setActionPopup('view');
                          }}>
                          <Eye className="w-4 h-4" />
                        </button>
                        {doc.isVerified == false && (
                          <button className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg"
                            onClick={() => {
                              setSelectedDoc(doc);
                              setActionPopup('upload');
                            }}>
                            <Edit className="w-4 h-4" />
                          </button>
                        )}
                      </div>
                    ) : (
                      <button className="inline-flex items-center px-3 py-2 bg-green-500 text-white text-sm rounded-lg hover:bg-green-600"
                        onClick={() => { setActionPopup('upload'); setSelectedDoc(doc) }}
                      >
                        <Upload className="w-4 h-4 mr-1" />
                        Upload
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Card>

      {actionPopup == 'upload' && (
        <Modal
          isOpen={true}
          onClose={() => { setActionPopup(null); setSelectedDoc(null) }}
          title={"Upload Document • " + selectedDoc.documentType.docTypeName}
          size="md"
        >
          <form onSubmit={handleUpload} className="space-y-4">
            <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center">
              <Upload className="w-10 h-10 mx-auto text-blue-500 mb-2" />
              <p className="text-gray-600 mb-2">Upload document</p>
              <Input
                type="file"
                onChange={(e) => setFile(e.target.files[0])}
                required
              />
              {file && (
                <p className="text-sm text-gray-500 mt-2">{file.name}</p>
              )}
            </div>

            <div className="flex justify-end gap-3 pt-4">
              <Button variant="secondary" onClick={() => { setActionPopup(null); setSelectedDoc(null) }}>
                Cancel
              </Button>
              <Button type="submit">
                Upload
              </Button>
            </div>
          </form>
        </Modal>
      )}


      {actionPopup === 'view' && selectedDoc && (
        <Modal
          isOpen={true}
          onClose={() => {
            setActionPopup(null);
            setSelectedDoc(null);
          }}
          title={`View Document • ${selectedDoc.documentType.docTypeName}`}
          size="lg"
        >
          <div className="space-y-4">
            <div className="border rounded-lg overflow-hidden">
              {selectedDoc.documentUrl.endsWith('.pdf') ? (
                <iframe
                  src={`${import.meta.env.VITE_API_URI}/${selectedDoc.documentUrl}`}
                  className="w-full h-[70vh]"
                  title="Document Preview"
                />
              ) : (
                <img
                  src={`${import.meta.env.VITE_API_URI}/${selectedDoc.documentUrl}`}
                  alt="Uploaded Document"
                  className="max-h-[70vh] mx-auto"
                />
              )}
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
