import React, {useEffect, useState} from 'react';
import AnimatedCounter from './components/AnimatedCounter';

function App() {
    const [profile, setProfile] = useState(null);
    const [repositories, setRepositories] = useState([]);
    const [experience, setExperience] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [activeSlide, setActiveSlide] = useState(0);
    const [animationDirection, setAnimationDirection] = useState(''); // 'next' or 'prev'

    useEffect(() => {
        // Fetch all data in parallel
        Promise.all([fetch('http://localhost:5248/api/profile').then(res => res.json()), fetch('http://localhost:5248/api/repositories').then(res => res.json()), fetch('http://localhost:5248/api/experience').then(res => res.json())])
            .then(([profileData, repositoriesData, experienceData]) => {
                setProfile(profileData);
                setRepositories(repositoriesData);
                setExperience(experienceData);
                setLoading(false);
            })
            .catch(err => {
                console.error('Error fetching data:', err);
                setError('Failed to load data from the backend. Please check the console for details.');
                setLoading(false);
            });
    }, []);

    // Add debugging to see what data is coming back
    useEffect(() => {
        if (repositories.length > 0) {
            console.log('Repositories data:', repositories);
        }
    }, [repositories]);

    // Custom carousel navigation functions
    const nextSlide = () => {
        setAnimationDirection('next');
        setActiveSlide((prev) => (prev + 1) % repositories.length);
    };

    const prevSlide = () => {
        setAnimationDirection('prev');
        setActiveSlide((prev) => (prev === 0 ? repositories.length - 1 : prev - 1));
    };

    if (loading) {
        return <div className="container mx-auto text-center my-20"><h2 className="text-2xl font-bold text-white">Loading...</h2></div>;
    }

    if (error) {
        return <div className="container mx-auto text-center my-20"><h2 className="text-2xl font-bold text-red-400">{error}</h2></div>;
    }

    return (<div className="App bg-gray-900 text-gray-100 min-h-screen">
        <main className="pb-16"> {/* Added padding bottom to account for fixed footer */}
            {/* Header Section */}
            <section className="text-center">
                <div className="container mx-auto py-8 max-w-md">
                    <h3 className="mt-auto mb-0 text-xl font-medium text-gray-300">Hello, I'm</h3>
                    <h1 className="text-4xl font-bold my-2 text-white">{profile.fullName}</h1>
                    <p className="text-lg text-gray-300">{profile.description}</p>
                    <p className="mt-4">
                        <a href={profile.githubUrl} className="inline-block px-6 py-2.5 mx-2 border border-blue-400 text-blue-400 font-medium rounded hover:bg-blue-700 hover:text-white transition-colors">GitHub</a>
                        <a href={profile.linkedinUrl} className="inline-block px-6 py-2.5 mx-2 border border-blue-400 text-blue-400 font-medium rounded hover:bg-blue-700 hover:text-white transition-colors">LinkedIn</a>
                    </p>
                </div>
            </section>

            {/* Repository Carousel */}
            <div id="carouselExampleDark" className="py-12 bg-gray-800 relative">
                {/* Carousel Items */}
                <div className="relative overflow-hidden w-full">
                    <div className="container mx-auto px-4">
                        {repositories.map((repository, pageIndex) => (
                            <div
                                key={pageIndex}
                                className={`transition-all duration-500 carousel-item ${
                                    pageIndex === activeSlide ? 'active' : ''
                                } ${animationDirection === 'next' ? 'slide-next' : 'slide-prev'}`}
                                style={{ 
                                    display: Math.abs(pageIndex - activeSlide) <= 1 || 
                                            (activeSlide === 0 && pageIndex === repositories.length - 1) || 
                                            (activeSlide === repositories.length - 1 && pageIndex === 0) 
                                                ? 'block' : 'none'
                                }}
                            >
                                <div className="flex flex-wrap -mx-4">
                                    {repository.map((repo, i) => (
                                        <div key={i} className="w-full md:w-1/3 px-4 mt-8 mb-8">
                                            {/* Minecraft-style repository box */}
                                            <div className="minecraft-box">
                                                <div className="minecraft-box-header flex justify-between items-center">
                                                    <h3 className="text-gray-200 font-medium">{repo.name}</h3>
                                                    <div className="flex">
                                                        <span className="commits-counter">
                                                            ~<AnimatedCounter
                                                                value={repo.commitCount}
                                                                triggerKey={pageIndex === activeSlide ? activeSlide : null}
                                                            /> commits
                                                        </span>
                                                        <span className="stars-counter">
                                                            <AnimatedCounter
                                                                value={repo.starCount || 0}
                                                                triggerKey={pageIndex === activeSlide ? activeSlide : null}
                                                            /> stars
                                                        </span>
                                                    </div>
                                                </div>
                                                <div className="minecraft-box-content">
                                                    <p className="text-gray-300 text-sm mb-2 flex-grow pl-1">{repo.description}</p> {/* Added pl-1 for padding-left */}
                                                    <div className="flex justify-between mb-2 items-center">
                                                        <span className="minecraft-tag">{repo.language}</span>
                                                        <small className="text-gray-400">{repo.getSimpleDate}</small>
                                                    </div>
                                                    <a
                                                        href={repo.htmlUrl}
                                                        className="minecraft-button w-full"
                                                    >
                                                        GitHub Repository Link
                                                    </a>
                                                </div>
                                                {/* The ::after pseudo-element will be added automatically */}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        ))}
                    </div>
                </div>

                {/* Carousel Indicators - Moved below repository content */}
                <div className="flex justify-center mt-6 space-x-2 items-center">
                    {repositories.map((_, index) => (
                        <button
                            key={index}
                            type="button"
                            onClick={() => {
                                setAnimationDirection(index > activeSlide ? 'next' : 'prev');
                                setActiveSlide(index);
                            }}
                            className={`rounded-none transition-all duration-300 ${
                                index === activeSlide 
                                ? 'w-5 h-5 bg-gray-300' 
                                : 'w-3 h-3 bg-gray-600 hover:bg-gray-500'
                            }`}
                            aria-current={index === activeSlide ? 'true' : 'false'}
                        />
                    ))}
                </div>

                {/* Carousel Controls */}
                <button
                    className="absolute top-1/2 left-2 -translate-y-1/2 bg-gray-700 p-3 shadow-md hover:bg-gray-600 focus:outline-none"
                    type="button"
                    onClick={prevSlide}
                >
                    <div
                        className="w-6 h-6 bg-white"
                        style={{
                            clipPath: 'polygon(75% 0%, 50% 0%, 25% 50%, 50% 100%, 75% 100%, 50% 50%)'
                        }}
                    />
                    <span className="sr-only">Previous</span>
                </button>

                <button
                    className="absolute top-1/2 right-2 -translate-y-1/2 bg-gray-700 p-3 shadow-md hover:bg-gray-600 focus:outline-none"
                    type="button"
                    onClick={nextSlide}
                >
                    <div
                        className="w-6 h-6 bg-white"
                        style={{
                            clipPath: 'polygon(25% 0%, 50% 0%, 75% 50%, 50% 100%, 25% 100%, 50% 50%)'
                        }}
                    />
                    <span className="sr-only">Next</span>
                </button>

            </div>

            {/* Experience Section */}
            <div className="container mx-auto px-4 py-8" id="experience">
                <h3 className="pb-2 border-b border-gray-700 text-2xl font-semibold text-white">Experience</h3>
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 py-8">
                    {experience.map((exp, index) => (
                        <div key={index} className="flex">
                            <div>
                                <h4 className="text-xl font-medium text-gray-200">{exp.title}</h4>
                                <p className="text-gray-400">{exp.getDescription}</p>
                            </div>
                        </div>
                    ))}
                </div>
            </div>

            {/* Footer */}
            <footer className="fixed bottom-0 inset-x-0 py-3 text-center border-t border-gray-700 bg-gray-800 text-lg text-gray-300">
                © {new Date().getFullYear()} Yattaw.ME
            </footer>
        </main>
    </div>);
}

export default App;
