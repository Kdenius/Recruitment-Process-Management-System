import { useEffect, useState } from "react";
import { Briefcase, Calendar, CheckCircle2, Plus, FileText, Clock, Eye, XCircle } from "lucide-react";
import { Modal } from "../components/common/modal";
import { Button } from "../components/common/button";
import { Select } from "../components/common/select";
import { Input } from "../components/common/input";
import { showToast } from "../components/common/toast";

export function HrDashboard() {
    const [applications, setApplications] = useState([]);
    const [selectedApp, setSelectedApp] = useState(null);
    const [requestedDocs, setRequestedDocs] = useState([]);
    const [availableDocs, setAvailableDocs] = useState([]);
    const [verifyPopup, setVerifyPopup] = useState(null);
    const [verifyRemarks, setVerifyRemarks] = useState("");
    const [newDocs, setNewDocs] = useState([]);
    const [allDocTypes, setAallDocTypes] = useState([])
    const [selectedDoc, setSelectedDoc] = useState(null);
    const [actionPopup, setActionPopup] = useState(null);


    const fetchData = async () => {
        try {
            const ret = await fetch(import.meta.env.VITE_API_URI + '/application/applications-with-documents', {
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${user.jwtToken}`
                },
            })
            const res = await ret.json();
            if (!ret.ok)
                throw new Error(res.message)
            setApplications(res);

            const ret2 = await fetch(import.meta.env.VITE_API_URI + '/document-types', {
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${user.jwtToken}`
                },
            })
            const res2 = await ret2.json();
            if (!ret2.ok)
                throw new Error(res2.message)
            setAallDocTypes(res2);
        } catch (e) {
            console.error(e);
        }
    }

    const handleDocumentSelect = (docTypeId) => {
        if (!docTypeId) return;

        const doc = allDocTypes.find(d => d.docTypeId === parseInt(docTypeId));

        if (
            doc &&
            !newDocs.some(d => d.docTypeId === doc.docTypeId) &&
            !requestedDocs.some(d => d.docTypeId === doc.docTypeId)
        ) {
            setNewDocs([...newDocs, doc]);
        }
    };


    const removeNewDoc = (docTypeId) => {
        setNewDocs(newDocs.filter(d => d.docTypeId !== docTypeId));
    };


    useEffect(() => {
        fetchData();
    }, []);

    useEffect(() => {
        if (selectedApp) {
            const requested = selectedApp.documents.filter(d => d.documentUrl || d.documentId);
            const available = allDocTypes.filter(
                dt => !requested.some(r => r.docTypeId === dt.docTypeId)
            );
            setRequestedDocs(requested);
            setAvailableDocs(available);
        }
    }, [selectedApp]);

    const handleRequestDoc = async () => {
        try{
            const ret = await fetch(import.meta.env.VITE_API_URI+'/document-verification/request',{
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${user.jwtToken}`
                },
                body: JSON.stringify({
                    applicationId: selectedApp.applicationId,
                    requiredDocTypeIds: newDocs.map(d => d.docTypeId)
                })
            });
            const res = await ret.json();
            if(!ret.ok)
                throw new Error(res);            
            showToast.success("Requested", res.message);
        }catch(e){
            console.error(e);
        }
    };

    const handleVerifyDoc = async (docId) => {
        try {
            const ret = await fetch(import.meta.env.VITE_API_URI + '/document-verification/verify', {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${user.jwtToken}`
                },
                body: JSON.stringify({
                    documentId: docId,
                    isVerified: true
                })
            })
            const res = await ret.json();
            if(!ret.ok)
                throw new Error(res);
            showToast.success("Document Updated", `Document Verified successfully`);
            setVerifyPopup(null);
            setVerifyRemarks("");
        } catch (e) {
            console.error(e);
        }
    };

    const getStatusColor = status => {
        if (status === "DocVerification") return "text-yellow-600";
        if (status === "DocsSubmitted") return "text-blue-600";
        if (status === "DocsVerified") return "text-green-600";
        if (status === "Selected") return "text-purple-600";
        return "text-gray-600";
    };

    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-3xl font-bold text-gray-800">HR Dashboard</h1>
                <p className="text-gray-600">Manage candidate documents and verification</p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {applications.map(app => (
                    <div
                        key={app.applicationId}
                        onClick={() => setSelectedApp(app)}
                        className="bg-gradient-to-r from-yellow-50 to-white rounded-lg border border-gray-200 p-5 hover:shadow-md hover:border-yellow-300 transition cursor-pointer"
                    >
                        <div className="flex items-start justify-between mb-3">
                            <div>
                                <h3 className="text-lg font-bold text-gray-800">{app.candidateName}</h3>
                                <p className="text-sm text-gray-600">{app.position}</p>
                            </div>
                            <div className={`text-sm font-bold ${getStatusColor(app.status)}`}>
                                {app.status}
                            </div>
                        </div>
                        <div className="flex flex-wrap gap-4 text-sm text-gray-600 mb-3">
                            <div className="flex items-center gap-1">
                                <Briefcase className="w-4 h-4" />
                                {app.position}
                            </div>
                            <div className="flex items-center gap-1">
                                <Calendar className="w-4 h-4" />
                                {new Date(app.createdAt).toLocaleDateString("en-GB")}
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            {selectedApp && (
                <Modal
                    isOpen={true}
                    onClose={() => setSelectedApp(null)}
                    title={`Application • ${selectedApp.candidateName}`}
                    size="xl"
                >
                    <div className="space-y-6">
                        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
                            <p className="font-semibold text-gray-800">
                                Position: <span className="font-normal">{selectedApp.position}</span>
                            </p>
                            <p className="text-sm text-gray-600">Status: {selectedApp.status}</p>
                        </div>

                        <div className="space-y-4">
                            <h3 className="font-bold text-gray-800 text-lg">Documents</h3>
                            {requestedDocs.length > 0 ? (
                                <table className="w-full">
                                    <thead className="bg-gradient-to-r from-green-50 to-green-100">
                                        <tr>
                                            <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">
                                                Document
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
                                        {requestedDocs.map(doc => (
                                            <tr key={doc.documentId || doc.docTypeId} className="hover:bg-gray-50">
                                                <td className="px-6 py-4 font-medium text-gray-800">
                                                    {doc.docTypeName}
                                                </td>

                                                <td className="px-6 py-4 text-gray-600 text-sm">
                                                    {doc.uploadedAt
                                                        ? new Date(doc.uploadedAt).toLocaleString("en-GB")
                                                        : "-"}
                                                </td>

                                                <td className="px-6 py-4">
                                                    {doc.uploadedAt ? (
                                                        doc.isVerified ? (
                                                            <span className="inline-flex items-center space-x-1 px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-700 border border-green-200">
                                                                <CheckCircle2 className="w-3 h-3" />
                                                                <span>Verified</span>
                                                            </span>
                                                        ) : (
                                                            <span className="inline-flex items-center space-x-1 px-2 py-1 rounded-full text-xs font-medium bg-yellow-100 text-yellow-700 border border-yellow-200">
                                                                <Clock className="w-3 h-3" />
                                                                <span>Pending Verification</span>
                                                            </span>
                                                        )
                                                    ) : (
                                                        <span className="inline-flex items-center space-x-1 px-2 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-700 border border-gray-200">
                                                            <Clock className="w-3 h-3" />
                                                            <span>Pending</span>
                                                        </span>
                                                    )}
                                                </td>

                                                <td className="px-6 py-4">
                                                    {doc.uploadedAt ? (
                                                        <div className="flex items-center space-x-2">
                                                            <button
                                                                className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg"
                                                                onClick={() => { setActionPopup('view'); setSelectedDoc(doc) }}
                                                            >
                                                                <Eye className="w-4 h-4" />
                                                            </button>
                                                            {!doc.isVerified && (
                                                                <button
                                                                    className="p-2 text-green-600 hover:bg-green-50 rounded-lg"
                                                                    onClick={() => { setVerifyPopup(doc.documentId) }}
                                                                >
                                                                    <CheckCircle2 className="w-4 h-4" />
                                                                </button>
                                                            )}
                                                        </div>
                                                    ) : (
                                                        <span className="text-gray-400">-</span>
                                                    )}
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <p className="text-gray-500">No documents requested yet.</p>
                            )}
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">Select Document</label>
                                <Select
                                    value=""
                                    onChange={(e) => handleDocumentSelect(e.target.value)}
                                    options={[
                                        { value: '', label: 'Select a document...' },
                                        ...allDocTypes
                                            .filter(
                                                doc =>
                                                    !requestedDocs.some(rd => rd.docTypeId === doc.docTypeId) &&
                                                    !newDocs.some(nd => nd.docTypeId === doc.docTypeId)
                                            )
                                            .map(doc => ({
                                                value: doc.docTypeId,
                                                label: doc.docTypeName
                                            }))
                                    ]}
                                />

                                <div className="flex flex-wrap gap-2 mt-3">
                                    {newDocs.length > 0 && (
                                        <div className="flex flex-wrap gap-2 mt-3">
                                            {newDocs.map(doc => (
                                                <span
                                                    key={doc.docTypeId}
                                                    className="inline-flex items-center bg-blue-50 text-blue-700 text-sm px-3 py-1 rounded-full border border-blue-200"
                                                >
                                                    {doc.docTypeName}
                                                    <button
                                                        type="button"
                                                        onClick={() => removeNewDoc(doc.docTypeId)}
                                                        className="ml-2 text-blue-500 hover:text-blue-800"
                                                    >
                                                        <XCircle className="w-4 h-4" />
                                                    </button>
                                                </span>
                                            ))}
                                        </div>
                                    )}
                                    {newDocs.length > 0 && (
                                        <div className="mt-4 flex justify-end">
                                            <Button size="sm" onClick={handleRequestDoc}>
                                                Request Documents
                                            </Button>
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>
                    </div>
                </Modal>
            )}
            {verifyPopup && (
                <Modal
                    isOpen={true}
                    onClose={() => setVerifyPopup(null)}
                    title="Verify Document"
                >
                    <div className="space-y-4">
                        <p>
                            Are you sure you want to verify this document?
                        </p>
                        <textarea
                            value={verifyRemarks}
                            onChange={(e) => setVerifyRemarks(e.target.value)}
                            placeholder="Optional remarks"
                            className="w-full border border-gray-300 rounded-lg p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
                            rows={3}
                        />
                        <div className="flex justify-end gap-3">
                            <Button
                                variant="secondary"
                                onClick={() => setVerifyPopup(null)}
                            >
                                Reupload
                            </Button>
                            <Button
                                variant="success"
                                onClick={() => handleVerifyDoc(verifyPopup)}
                            >
                                Verify
                            </Button>
                        </div>
                    </div>
                </Modal>
            )}

            {actionPopup === 'view' && selectedDoc && (
                <Modal
                    isOpen={true}
                    onClose={() => {
                        setActionPopup(null);
                        setSelectedDoc(null);
                    }}
                    title={`View Document • ${selectedDoc.docTypeName}`}
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
