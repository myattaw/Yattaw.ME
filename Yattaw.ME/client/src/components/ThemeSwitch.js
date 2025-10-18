import React from 'react';

export const ThemeSwitch = ({ isMinecraft, onToggle }) => {
    return (
        <div className="fixed top-4 right-4 z-50">
            <button
                onClick={onToggle}
                className={`theme-switch ${isMinecraft ? 'minecraft' : 'modern'}`}
                aria-label="Toggle theme"
            >
                <div className="theme-switch-track">
                    <div className={`theme-switch-thumb ${isMinecraft ? 'active' : ''}`}>
                        <img
                            src={
                                isMinecraft
                                    ? '/icons/theme-minecraft-toggle.svg'
                                    : '/icons/theme-modern-toggle.svg'
                            }
                            alt={isMinecraft ? 'Minecraft theme icon' : 'Modern theme icon'}
                            className="w-4 h-4"
                        />
                    </div>
                </div>

                <span className="theme-switch-label">
          {isMinecraft ? 'Minecraft' : 'Modern'}
        </span>
            </button>
        </div>
    );
};
