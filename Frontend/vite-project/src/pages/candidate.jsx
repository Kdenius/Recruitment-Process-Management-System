import { useState } from 'react';
import { Card } from '../components/common/Card';
import { Button } from '../components/common/button';
import { Modal } from '../components/common/modal';
import { Input } from '../components/common/input';
import { Select } from '../components/common/select';
import { showToast } from '../components/common/toast';
import { Users, Plus, Search, Filter, Eye, Upload, FileSpreadsheet } from 'lucide-react';

export function Candidates() {
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [uploadMethod, setUploadMethod] = useState('manual');
  const [searchTerm, setSearchTerm] = useState('');
  const [resumeFile, setResumeFile] = useState(null);
  const [parsedData, setParsedData] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleAddCandidate = async (e) => {
    e.preventDefault();
    setIsLoading(true);

    let candidateData = parsedData;

    // If file is CV, parse the resume first
    if (uploadMethod === 'cv' && resumeFile) {
      // Submit file to resume parser
      const formData = new FormData();
      if (!resumeFile)
        console.log("aaj null che")
      formData.append('file', resumeFile);

      try {
        const response = await fetch('http://localhost:8000/parse-resume', {
          method: 'POST',
          body: formData
        });


        if (response.ok) {
          const data = await response.json();
          console.log('data from parser');
          console.log(data);
          setParsedData(data);
          candidateData = data;
        } else {
          throw new Error('Failed to parse resume');
        }
      } catch (error) {
        showToast.error('Resume Parsing Failed', error.message);
        setIsLoading(false);
        return;
      }
    }

    const candidateFormData = new FormData();
    candidateFormData.append('Name', candidateData.name);
    candidateFormData.append('Email', candidateData.email);
    candidateFormData.append('PhoneNumber', candidateData.phone_number);
    candidateFormData.append('Age', candidateData.age || 0);
    // candidateFormData.append('Languages',  []);
    // candidateFormData.append('CandidateSkillIds', [1,2]);
    candidateFormData.append('ClientUrl', 'http://localhost:3000/candidate/' + candidateData.name); // Or whatever URL you want to associate
    candidateFormData.append('resume', resumeFile);
    const skillIds = [1, 2];
    skillIds.forEach(id => {
      candidateFormData.append('CandidateSkillIds', id);
    });
    
    const languages = candidateData.languages || [];
    languages.forEach(lang => {
      candidateFormData.append('Languages', lang);
    });
    try {
      const apiResponse = await fetch(import.meta.env.VITE_API_URI + '/candidate', {
        method: 'POST',
        body: candidateFormData,
      });

      if (apiResponse.ok) {
        showToast.success('Candidate Added', 'Candidate profile created successfully');
        setIsAddModalOpen(false); // Close the modal
      } else {
        throw new Error('Failed to submit candidate data');
      }
    } catch (error) {
      showToast.error('Submission Failed', error.message);
    }

    setIsLoading(false);
  };
  //   const handleAddCandidate = (e) => {
  //     e.preventDefault();
  //     setIsAddModalOpen(false);
  //     if (uploadMethod === 'manual') {
  //       showToast.success('Candidate Added', 'New candidate profile has been created successfully');
  //     } else if (uploadMethod === 'cv') {
  //       showToast.success('CV Processed', 'Candidate profile created from CV successfully');
  //     } else {
  //       showToast.success('Bulk Import Complete', 'Multiple candidates have been imported successfully');
  //     }
  //   };

  //static cnadidate for style
  const candidates = [
    { id: '1', name: 'John Doe', email: 'john@example.com', phone: '+1234567890', experience: 5, status: 'screening', appliedFor: 'Senior Developer', college: 'MIT' },
    { id: '2', name: 'Jane Smith', email: 'jane@example.com', phone: '+1234567891', experience: 3, status: 'interview', appliedFor: 'UI/UX Designer', college: 'Stanford' },
    { id: '3', name: 'Michael Brown', email: 'michael@example.com', phone: '+1234567892', experience: 7, status: 'selected', appliedFor: 'DevOps Engineer', college: 'Berkeley' },
    { id: '4', name: 'Sarah Johnson', email: 'sarah@example.com', phone: '+1234567893', experience: 4, status: 'shortlisted', appliedFor: 'Frontend Developer', college: 'Harvard' },
  ];

  const getStatusBadge = (status) => {
    const styles = {
      uploaded: 'bg-blue-100 text-blue-700 border-blue-200',
      screening: 'bg-yellow-100 text-yellow-700 border-yellow-200',
      shortlisted: 'bg-purple-100 text-purple-700 border-purple-200',
      interview: 'bg-orange-100 text-orange-700 border-orange-200',
      selected: 'bg-green-100 text-green-700 border-green-200',
      rejected: 'bg-red-100 text-red-700 border-red-200',
      hold: 'bg-gray-100 text-gray-700 border-gray-200',
    };
    return (
      <span className={`inline-flex px-2 py-1 rounded-full text-xs font-medium border ${styles[status]}`}>
        {status.charAt(0).toUpperCase() + status.slice(1)}
      </span>
    );
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setResumeFile(file);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-800">Candidates</h1>
          <p className="text-gray-600 mt-1">Manage candidate profiles and applications</p>
        </div>
        <Button onClick={() => setIsAddModalOpen(true)} className="flex items-center space-x-2">
          <Plus className="w-5 h-5" />
          <span>Add Candidate</span>
        </Button>
      </div>

      <Card>
        <div className="flex flex-col sm:flex-row gap-4 mb-6">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
            <input
              type="text"
              placeholder="Search candidates..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
            />
          </div>
          <div className="flex items-center space-x-2">
            <Filter className="w-5 h-5 text-gray-400" />
            <Select
              options={[
                { value: 'all', label: 'All Status' },
                { value: 'screening', label: 'Screening' },
                { value: 'interview', label: 'Interview' },
                { value: 'selected', label: 'Selected' },
              ]}
            />
          </div>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gradient-to-r from-green-50 to-green-100">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Candidate</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Contact</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Experience</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">College</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Applied For</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {candidates.map((candidate) => (
                <tr key={candidate.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4">
                    <div className="flex items-center space-x-3">
                      <div className="w-10 h-10 bg-gradient-to-br from-green-400 to-green-500 rounded-full flex items-center justify-center">
                        <span className="text-white font-semibold">
                          {candidate.name.split(' ').map(n => n[0]).join('')}
                        </span>
                      </div>
                      <div>
                        <p className="font-semibold text-gray-800">{candidate.name}</p>
                        <p className="text-sm text-gray-500">ID: {candidate.id}</p>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <p className="text-sm text-gray-700">{candidate.email}</p>
                    <p className="text-xs text-gray-500">{candidate.phone}</p>
                  </td>
                  <td className="px-6 py-4 text-gray-700">{candidate.experience} years</td>
                  <td className="px-6 py-4 text-gray-700">{candidate.college}</td>
                  <td className="px-6 py-4 text-gray-700">{candidate.appliedFor}</td>
                  <td className="px-6 py-4">{getStatusBadge(candidate.status)}</td>
                  <td className="px-6 py-4">
                    <button className="p-2 text-green-600 hover:bg-green-50 rounded-lg transition-colors">
                      <Eye className="w-4 h-4" />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Card>

      <Modal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        title="Add New Candidate"
        size="lg"
      >
        <div className="space-y-4">
          <div className="flex space-x-4 mb-6">
            <button
              onClick={() => setUploadMethod('manual')}
              className={`flex-1 p-4 rounded-lg border-2 transition-all ${uploadMethod === 'manual' ? 'border-green-500 bg-green-50' : 'border-gray-200 hover:border-gray-300'}`}
            >
              <Users className="w-8 h-8 mx-auto mb-2 text-green-600" />
              <p className="font-semibold">Manual Entry</p>
            </button>
            <button
              onClick={() => setUploadMethod('cv')}
              className={`flex-1 p-4 rounded-lg border-2 transition-all ${uploadMethod === 'cv' ? 'border-green-500 bg-green-50' : 'border-gray-200 hover:border-gray-300'}`}
            >
              <Upload className="w-8 h-8 mx-auto mb-2 text-green-600" />
              <p className="font-semibold">Upload CV</p>
            </button>
            <button
              onClick={() => setUploadMethod('excel')}
              className={`flex-1 p-4 rounded-lg border-2 transition-all ${uploadMethod === 'excel' ? 'border-green-500 bg-green-50' : 'border-gray-200 hover:border-gray-300'}`}
            >
              <FileSpreadsheet className="w-8 h-8 mx-auto mb-2 text-green-600" />
              <p className="font-semibold">Bulk Upload</p>
            </button>
          </div>

          {uploadMethod === 'manual' && (
            <form onSubmit={handleAddCandidate} className="space-y-4">
              <Input label="Full Name" placeholder="Enter candidate name" required />
              <div className="grid grid-cols-2 gap-4">
                <Input label="Email" type="email" placeholder="email@example.com" required />
                <Input label="Phone" type="tel" placeholder="+1234567890" required />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <Input label="Years of Experience" type="number" placeholder="5" required />
                <Input label="College/University" placeholder="University name" />
              </div>
              <Select
                label="Applied Position"
                options={[
                  { value: '', label: 'Select Position' },
                  { value: '1', label: 'Senior Developer' },
                  { value: '2', label: 'UI/UX Designer' },
                ]}
                required
              />
              <div className="flex justify-end space-x-3 pt-4">
                <Button variant="secondary" onClick={() => setIsAddModalOpen(false)}>
                  Cancel
                </Button>
                <Button type="submit">Add Candidate</Button>
              </div>
            </form>
          )}

          {uploadMethod === 'cv' && (
            <form onSubmit={handleAddCandidate} className="space-y-4">
              <div className="border-2 border-dashed border-gray-300 rounded-lg p-8 text-center">
                <Upload className="w-12 h-12 mx-auto text-gray-400 mb-4" />
                <p className="text-gray-600 mb-2">Drop CV file here or click to browse</p>
                <Input
                  type="file"
                  accept=".pdf, .doc, .docx"
                  onChange={handleFileChange}
                  className="mt-2"
                />
                {resumeFile && <p className="text-sm text-gray-500">{resumeFile.name}</p>}
              </div>
              <div className="flex justify-end space-x-3 pt-4">
                <Button variant="secondary" onClick={() => setIsAddModalOpen(false)}>
                  Cancel
                </Button>
                <Button type="submit">Upload & Process</Button>
              </div>
            </form>
          )}

          {uploadMethod === 'excel' && (
            <form onSubmit={handleAddCandidate} className="space-y-4">
              <div className="border-2 border-dashed border-gray-300 rounded-lg p-8 text-center">
                <FileSpreadsheet className="w-12 h-12 mx-auto text-gray-400 mb-4" />
                <p className="text-gray-600 mb-2">Upload Excel file with candidate data</p>
                <Button variant="secondary" size="sm" type="button">Choose File</Button>
              </div>
              <div className="flex justify-end space-x-3 pt-4">
                <Button variant="secondary" onClick={() => setIsAddModalOpen(false)}>
                  Cancel
                </Button>
                <Button type="submit">Upload & Import</Button>
              </div>
            </form>
          )}
        </div>
      </Modal>
    </div>
  );
}
