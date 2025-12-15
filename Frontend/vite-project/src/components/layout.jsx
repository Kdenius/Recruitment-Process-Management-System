import { Navbar } from './common/Navbar';
import {Sidebar} from './common/sidebar';

export function Layout({ children }) {
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100">
      <Navbar />
      <div className="flex">
        <Sidebar/>
        <main className="flex-1 p-8">
          {children}
        </main>
      </div>
    </div>
  );
}
