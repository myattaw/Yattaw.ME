import React, { useState, useRef, useEffect } from "react";
import ReactDOM from "react-dom";
import AnimatedCounter from "./AnimatedCounter";
import { ReadmePreview } from "./ReadmePreview";

const HoverCard = ({ children }) => {
    const [open, setOpen] = useState(false);
    const [coords, setCoords] = useState({ top: 0, left: 0 });
    const triggerRef = useRef(null);
    const timeoutRef = useRef(null);

    const handleMouseEnter = () => {
        if (timeoutRef.current) clearTimeout(timeoutRef.current);
        if (triggerRef.current) {
            const rect = triggerRef.current.getBoundingClientRect();
            // Position the hover card at the vertical center of the header text
            setCoords({
                top: rect.top + window.scrollY + rect.height / 2, // Use scrollY for correct placement
                left: rect.left + window.scrollX,
            });
        }
        setOpen(true);
    };

    const handleMouseLeave = () => {
        timeoutRef.current = setTimeout(() => setOpen(false), 200);
    };

    return (
        <span
            ref={triggerRef}
            style={{ position: "relative", display: "inline-block" }}
            onMouseEnter={handleMouseEnter}
            onMouseLeave={handleMouseLeave}
        >
      {React.Children.map(children, (child) =>
          React.cloneElement(child, {
              open,
              coords,
              onKeepOpen: () => {
                  if (timeoutRef.current) clearTimeout(timeoutRef.current);
                  setOpen(true);
              },
              onClose: handleMouseLeave,
          })
      )}
    </span>
    );
};

const HoverCardTrigger = ({ children }) => children;

// --- Corrected positioning + fade animation ---
const HoverCardContent = ({
    children,
    open,
    coords,
    onKeepOpen,
    onClose,
    isMinecraftTheme,
}) => {
    const [visible, setVisible] = useState(false);

    useEffect(() => {
        if (open) setVisible(true);
        else {
            const timer = setTimeout(() => setVisible(false), 150);
            return () => clearTimeout(timer);
        }
    }, [open]);

    if (!visible) return null;

    // Ensure parent container has a background by using your CSS class
    // Remove any conflicting inline background styles
    return ReactDOM.createPortal(
        <div
            onMouseEnter={onKeepOpen}
            onMouseLeave={onClose}
            className={`hovercard-content ${isMinecraftTheme ? "hovercard-minecraft" : "hovercard-modern"}`}
            style={{
                top: `${coords.top}px`,
                left: `${coords.left}px`,
                position: "absolute",
                zIndex: 9999,
            }}
        >
            {children}
        </div>,
        document.body
    );
};

// --- Repository Card ---
const RepositoryCard = ({
                            repository,
                            isActive,
                            activeSlide,
                            isMinecraftTheme,
                        }) => {
    const [readmeContent, setReadmeContent] = useState(null);

    useEffect(() => {
        const raw = repository.readMeContent;
        if (!raw) {
            setReadmeContent(null);
            return;
        }
        try {
            if (/^[A-Za-z0-9+/=\n]+$/.test(raw)) {
                const decoded = new TextDecoder().decode(
                    Uint8Array.from(atob(raw.replace(/\n/g, "")), (c) => c.charCodeAt(0))
                );
                setReadmeContent(decoded);
            } else {
                setReadmeContent(raw);
            }
        } catch {
            setReadmeContent(raw);
        }
    }, [repository]);

    return (
        <div
            className={
                isMinecraftTheme
                    ? "minecraft-box"
                    : "rounded-lg shadow-lg bg-gray-700"
            }
            style={{ overflow: "visible" }}
        >
            <div
                className={
                    isMinecraftTheme
                        ? "minecraft-box-header flex justify-between items-center"
                        : "flex justify-between items-center p-3 border-b border-gray-600"
                }
            >
                <HoverCard>
                    <HoverCardTrigger>
                        <h3 className="text-gray-200 cursor-help hover:text-white transition-colors">
                            {repository.name}
                        </h3>
                    </HoverCardTrigger>
                    <HoverCardContent isMinecraftTheme={isMinecraftTheme}>
                        {readmeContent ? (
                            <ReadmePreview
                                content={readmeContent}
                                repositoryUrl={repository.htmlUrl}
                                isMinecraftTheme={isMinecraftTheme}
                            />
                        ) : (
                            <div className="text-gray-400">README not available</div>
                        )}
                    </HoverCardContent>
                </HoverCard>

                <div className="flex gap-2">
          <span className="commits-counter text-xs text-gray-400">
            ~
            <AnimatedCounter
                value={repository.commitCount}
                triggerKey={isActive ? activeSlide : null}
            />{" "}
              commits
          </span>
                    <span className="stars-counter text-xs text-yellow-400 ml-2">
            <AnimatedCounter
                value={repository.starCount || 0}
                triggerKey={isActive ? activeSlide : null}
            />{" "}
                        ★
          </span>
                </div>
            </div>

            <div className={isMinecraftTheme ? "minecraft-box-content" : "p-4"}>
                <p className="text-gray-300 text-sm mb-2 flex-grow pl-1">
                    {repository.description}
                </p>
                <div className="flex justify-between mb-2 items-center">
          <span
              className={
                  isMinecraftTheme
                      ? "minecraft-tag"
                      : "bg-gray-600 px-2 py-1 rounded text-xs"
              }
          >
            {repository.language}
          </span>
                    <small className="text-gray-400">
                        {repository.getSimpleDate}
                    </small>
                </div>
                <a
                    href={repository.htmlUrl}
                    className={
                        isMinecraftTheme
                            ? "minecraft-button w-full"
                            : "block mt-2 text-blue-400 hover:underline"
                    }
                    target="_blank"
                    rel="noopener noreferrer"
                >
                    GitHub Repository Link
                </a>
            </div>
        </div>
    );
};

export default RepositoryCard;
