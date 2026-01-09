import React, { useEffect } from 'react'
import { useState, useMemo } from 'react';
import {
  Briefcase,
  MapPin,
  DollarSign,
  Users,
  Search,
  CheckCircle2,
  X,
  BookOpen,
  Award,
  IndianRupee,
} from 'lucide-react';
import { Modal } from '../components/common/modal';
import { Input } from '../components/common/input';
import { Select } from '../components/common/select';
import { Card } from '../components/common/Card';
import { useAuth } from '../context/AuthContext';
import { showToast } from '../components/common/toast';



const SAMPLE_JOBS = [
  {
    id: '1',
    title: 'Senior Frontend Developer',
    company: 'Tech Startup Inc',
    location: 'San Francisco, CA',
    type: 'Full-time',
    salary: '$120k - $160k',
    applicants: 24,
    requiredSkills: ['React', 'TypeScript', 'Tailwind CSS', 'REST APIs'],
    preferredSkills: ['Next.js', 'GraphQL', 'Testing'],
    description:
      'We are looking for an experienced Frontend Developer to join our growing team. You will work on building scalable web applications using modern technologies.',
    posted: '2 days ago',
  }
];

export function CandidateDashboard() {
  const { user } = useAuth();

  const [searchQuery, setSearchQuery] = useState('');
  const [filterType, setFilterType] = useState('');
  const [filterLocation, setFilterLocation] = useState('');
  const [selectedJob, setSelectedJob] = useState(null);
  const [appliedJobs, setAppliedJobs] = useState([]);
  const [jobs, setJobs] = useState([]);

  const candidateSkills = user.candidateSkills.map(sk => sk.skillName)
  // user?.skills || ['React', 'TypeScript', 'Node.js', 'REST APIs', 'Docker'];

  console.log(user);
  useEffect(() => {
    const fetchPostion = async () => {
      try {
        const ret = await fetch(import.meta.env.VITE_API_URI + '/position');
        const data = await ret.json();
        if (!ret.ok)
          throw new Error("Issue in fetching position");
        console.log(data);
        const genData = data.map(p => ({
          ...p,
          requiredSkills: p.positionSkills.filter(sk => sk.isRequired).map(sk => sk.roleName),
          preferredSkills: p.positionSkills.filter(sk => !sk.isRequired).map(sk => sk.roleName)
        }))
        // job.positionSkills.filter(sk => sk.isRequired).map(sk => sk.roleName)
        console.log(genData)
        setJobs(genData);
      } catch (e) { }
      finally { }
    }

    const fetchAppliedJobs = async () => {
      try{
        const res = await fetch(
          `${import.meta.env.VITE_API_URI}/candidate/${user.candidateId}/applications`
        );
        const data = await res.json();
      if(!res.ok)
          throw new Error(data.message)
        setAppliedJobs(data.map(a => a.positionId));
      }catch(e){ console.error(e.message)}
    };
    fetchPostion();
    fetchAppliedJobs();
  }, [])
  const filteredJobs = useMemo(() => {
    return jobs.filter((job) => {
      const matchesSearch =
        job.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
        job.company.toLowerCase().includes(searchQuery.toLowerCase());

      const matchesType = !filterType || job.type === filterType;
      const matchesLocation =
        !filterLocation ||
        job.location.toLowerCase().includes(filterLocation.toLowerCase());

      return matchesSearch && matchesType && matchesLocation;
    });
  }, [jobs, searchQuery, filterType, filterLocation]);

  const calculateMatchPercentage = (requiredSkills) => {
    if (!requiredSkills.length) return 0;

    const matched = requiredSkills.filter((skill) =>
      candidateSkills.some(
        (cs) => cs.toLowerCase() === skill.toLowerCase()
      )
    ).length;

    return Math.round((matched / requiredSkills.length) * 100);
  };

  const handleApply = async (positionId) => {
    try {
      const res = await fetch(
        `${import.meta.env.VITE_API_URI}/candidate/apply`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            candidateId: user.candidateId,
            positionId: positionId,
          }),
        }
      );
      if (!res.ok) throw new Error('Apply failed');
      setAppliedJobs((prev) => [...prev, positionId]);
      showToast.success("Applied", "your application has been submited")
    } catch (e) {
      showToast.error('Applied Failed', e.message);
    }
  };

  const handleWithdraw = (jobId) => {
    setAppliedJobs(appliedJobs.filter((id) => id !== jobId));
  };

  return (
    <div className="space-y-6">
      <div className="bg-gradient-to-r from-blue-50 to-blue-100 rounded-xl p-8 border border-blue-200">
        <div className="flex items-start justify-between">
          <div>
            <h1 className="text-3xl font-bold text-gray-800 mb-2">
              Welcome, {user?.name}!
            </h1>
            <p className="text-gray-600">Explore job opportunities that match your skills</p>
          </div>
          <div className="w-16 h-16 bg-gradient-to-br from-blue-400 to-blue-500 rounded-full flex items-center justify-center shadow-lg">
            <Award className="w-8 h-8 text-white" />
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="bg-white rounded-xl shadow-md p-6 border border-gray-200">
          <div className="flex items-center space-x-3 mb-2">
            <div className="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
              <Award className="w-5 h-5 text-blue-600" />
            </div>
            <p className="text-sm text-gray-600">Skills</p>
          </div>
          <p className="text-2xl font-bold text-gray-800">{candidateSkills.length}</p>
        </div>

        <div className="bg-white rounded-xl shadow-md p-6 border border-gray-200">
          <div className="flex items-center space-x-3 mb-2">
            <div className="w-10 h-10 bg-green-100 rounded-lg flex items-center justify-center">
              <CheckCircle2 className="w-5 h-5 text-green-600" />
            </div>
            <p className="text-sm text-gray-600">Applications</p>
          </div>
          <p className="text-2xl font-bold text-gray-800">{appliedJobs.length}</p>
        </div>

        <div className="bg-white rounded-xl shadow-md p-6 border border-gray-200">
          <div className="flex items-center space-x-3 mb-2">
            <div className="w-10 h-10 bg-purple-100 rounded-lg flex items-center justify-center">
              <Briefcase className="w-5 h-5 text-purple-600" />
            </div>
            <p className="text-sm text-gray-600">Open Positions</p>
          </div>
          <p className="text-2xl font-bold text-gray-800">{jobs.length}</p>
        </div>
      </div>

      <Card title="Your Skills">
        <div className="flex flex-wrap gap-2">
          {candidateSkills.map((skill, index) => (
            <span
              key={index}
              className="px-3 py-1 bg-gradient-to-r from-blue-100 to-blue-50 text-blue-700 rounded-full text-sm font-medium border border-blue-200"
            >
              {skill}
            </span>
          ))}
        </div>
      </Card>

      <div className="space-y-4">
        <div>
          <h2 className="text-xl font-bold text-gray-800 mb-4">Explore Job Opportunities</h2>
          <div className="bg-white rounded-xl shadow-md p-6 border border-gray-200">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
              <div className="relative">
                <Search className="absolute left-3 top-3 w-5 h-5 text-gray-400" />
                <Input
                  placeholder="Search jobs..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="pl-10"
                />
              </div>

              <Select
                value={filterType}
                onChange={(e) => setFilterType(e.target.value)}
                placeholder="Filter by type"
                options={[
                  { value: "", label: "All Types" },
                  { value: "Full-time", label: "Full-time" },
                  { value: "Contract", label: "Contract" },
                  { value: "Part-time", label: "Part-time" }
                ]}
              />


              <Select
                value={filterLocation}
                onChange={(e) => setFilterLocation(e.target.value)}
                placeholder="Filter by location"
                options={[
                  { value: "", label: "All Locations" },
                  { value: "Remote", label: "Remote" },
                  { value: "Contract", label: "Contract" },
                  { value: "Part-time", label: "Part-time" }
                ]}
              />
            </div>

            <div className="grid grid-cols-1 gap-4">
              {filteredJobs.map((job) => {

                const matchPercentage = calculateMatchPercentage(job.requiredSkills);
                const isApplied = appliedJobs.includes(job.positionId);
                const matchColor =
                  matchPercentage >= 75 ? 'text-green-600' : matchPercentage >= 50 ? 'text-yellow-600' : 'text-red-600';

                return (
                  <div
                    key={job.positionId}
                    onClick={() => setSelectedJob(job)}
                    className="bg-gradient-to-r from-blue-50 to-white rounded-lg border border-gray-200 p-5 hover:shadow-md hover:border-blue-300 transition-all cursor-pointer"
                  >
                    <div className="flex items-start justify-between mb-3">
                      <div>
                        <h3 className="text-lg font-bold text-gray-800">{job.title}</h3>
                        <p className="text-sm text-gray-600">{job.company || 'Company'}</p>
                      </div>
                      <div className={`text-2xl font-bold ${matchColor}`}>{matchPercentage}%</div>
                    </div>

                    <div className="flex flex-wrap gap-4 mb-3 text-sm text-gray-600">
                      <div className="flex items-center space-x-1">
                        <MapPin className="w-4 h-4" />
                        <span>{job.location || 'Location'}</span>
                      </div>
                      <div className="flex items-center space-x-1">
                        <Briefcase className="w-4 h-4" />
                        <span>{job.type || 'Job Type'}</span>
                      </div>
                      <div className="flex items-center space-x-1">
                        <IndianRupee className="w-4 h-4" />
                        <span>{job.baseSalary + '-' + job.maxSalary || 'Pagar'}</span>
                      </div>
                      <div className="flex items-center space-x-1">
                        <Users className="w-4 h-4" />
                        <span>{job.applicants} applicants</span>
                      </div>
                    </div>

                    <div className="flex flex-wrap gap-1 mb-3">
                      {job.requiredSkills.slice(0, 3).map((skill, index) => {
                        const hasSkill = candidateSkills.some(
                          (cs) => cs.toLowerCase() === skill.toLowerCase()
                        );
                        return (
                          <span
                            key={index}
                            className={`px-2 py-1 rounded text-xs font-medium ${hasSkill
                              ? 'bg-green-100 text-green-700'
                              : 'bg-gray-100 text-gray-600'
                              }`}
                          >
                            {skill}
                          </span>
                        );
                      })}
                      {job.requiredSkills.length > 3 && (
                        <span className="px-2 py-1 text-xs text-gray-500">+{job.requiredSkills.length - 3} more</span>
                      )}
                    </div>

                    {isApplied && (
                      <div className="flex items-center space-x-2 text-green-600 text-sm font-medium">
                        <CheckCircle2 className="w-4 h-4" />
                        <span>Applied</span>
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
          </div>
        </div>
      </div>

      {selectedJob && (
        <Modal
          isOpen={true}
          onClose={() => setSelectedJob(null)}
          title={selectedJob.title}
        >
          <div className="space-y-6">
            <div>
              <div className="flex items-start justify-between mb-4">
                <div>
                  <h3 className="text-xl font-bold text-gray-800">{selectedJob.title}</h3>
                  <p className="text-gray-600">{selectedJob.status}</p>
                </div>
                <div className={`text-3xl font-bold ${calculateMatchPercentage(selectedJob.requiredSkills) >= 75
                  ? 'text-green-600'
                  : calculateMatchPercentage(selectedJob.requiredSkills) >= 50
                    ? 'text-yellow-600'
                    : 'text-red-600'
                  }`}>
                  {calculateMatchPercentage(selectedJob.requiredSkills)}% Match
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4 mb-4">
                <div className="flex items-center space-x-2 text-gray-700">
                  <MapPin className="w-5 h-5 text-blue-600" />
                  <span>{selectedJob.location}</span>
                </div>
                <div className="flex items-center space-x-2 text-gray-700">
                  <Briefcase className="w-5 h-5 text-blue-600" />
                  <span>{selectedJob.type}</span>
                </div>
                <div className="flex items-center space-x-2 text-gray-700">
                  <DollarSign className="w-5 h-5 text-blue-600" />
                  <span>{selectedJob.baseSalary + ' - ' + selectedJob.maxSalary}</span>
                </div>
                <div className="flex items-center space-x-2 text-gray-700">
                  <Users className="w-5 h-5 text-blue-600" />
                  <span>{selectedJob.applicants} applicants</span>
                </div>
              </div>
            </div>

            <div className="border-t pt-4">
              <p className="text-gray-700">{selectedJob.description}</p>
            </div>

            <div>
              <h4 className="font-bold text-gray-800 mb-3 flex items-center space-x-2">
                <BookOpen className="w-5 h-5 text-blue-600" />
                <span>Required Skills</span>
              </h4>
              <div className="space-y-2">
                {selectedJob.requiredSkills.map((skill, index) => {
                  const hasSkill = candidateSkills.some(
                    (cs) => cs.toLowerCase() === skill.toLowerCase()
                  );
                  return (
                    <div key={index} className="flex items-center space-x-2">
                      {hasSkill ? (
                        <CheckCircle2 className="w-5 h-5 text-green-600" />
                      ) : (
                        <div className="w-5 h-5 rounded border border-gray-300"></div>
                      )}
                      <span className={hasSkill ? 'text-green-700 font-medium' : 'text-gray-700'}>
                        {skill}
                      </span>
                    </div>
                  );
                })}
              </div>
            </div>

            {selectedJob.preferredSkills.length > 0 && (
              <div>
                <h4 className="font-bold text-gray-800 mb-3">Preferred Skills</h4>
                <div className="space-y-2">
                  {selectedJob.preferredSkills.map((skill, index) => {
                    const hasSkill = candidateSkills.some(
                      (cs) => cs.toLowerCase() === skill.toLowerCase()
                    );
                    return (
                      <div key={index} className="flex items-center space-x-2">
                        {hasSkill ? (
                          <CheckCircle2 className="w-5 h-5 text-blue-600" />
                        ) : (
                          <div className="w-5 h-5 rounded border border-gray-300"></div>
                        )}
                        <span className={hasSkill ? 'text-blue-700 font-medium' : 'text-gray-700'}>
                          {skill}
                        </span>
                      </div>
                    );
                  })}
                </div>
              </div>
            )}

            <div className="border-t pt-4 flex gap-3">
              {appliedJobs.includes(selectedJob.positionId) ? (
                <button
                  onClick={() => handleWithdraw(selectedJob.positionId)}
                  className="flex-1 px-4 py-2 bg-red-500 hover:bg-red-600 text-white rounded-lg font-medium transition-colors flex items-center justify-center space-x-2"
                >
                  <X className="w-5 h-5" />
                  <span>Withdraw Application</span>
                </button>
              ) : (
                <button
                  onClick={() => handleApply(selectedJob.positionId)}
                  className="flex-1 px-4 py-2 bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white rounded-lg font-medium transition-all"
                >
                  Apply Now
                </button>
              )}
              <button
                onClick={() => setSelectedJob(null)}
                className="px-4 py-2 bg-gray-200 hover:bg-gray-300 text-gray-800 rounded-lg font-medium transition-colors"
              >
                Close
              </button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
