import React from 'react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { Prism as SyntaxHighlighter } from 'react-syntax-highlighter';
import { darcula, vscDarkPlus } from 'react-syntax-highlighter/dist/esm/styles/prism';
import { FileText } from 'lucide-react';

export const ReadmePreview = ({ content, isMinecraftTheme = true }) => {
    
    const previewContent = content.slice(0, 2000);
    const isTruncated = content.length > 2000;

    return (
        <div className="space-y-3">
            <div className="flex items-center gap-2 text-gray-300 mb-3 pb-2 border-b border-gray-700">
                <FileText className="w-4 h-4" />
                <span className="font-medium">README Preview</span>
            </div>

            <div
                className={`readme-preview ${isMinecraftTheme ? 'minecraft' : 'modern'} hovercard-content`}
                style={{ maxHeight: '500px', overflowY: 'auto' }}
            >
                <ReactMarkdown
                    remarkPlugins={[remarkGfm]}
                    components={{
                        h1: ({ children }) => <h1 className="readme-h1">{children}</h1>,
                        h2: ({ children }) => <h2 className="readme-h2">{children}</h2>,
                        h3: ({ children }) => <h3 className="readme-h3">{children}</h3>,
                        p: ({ children }) => <p className="readme-p">{children}</p>,
                        ul: ({ children }) => <ul className="readme-ul">{children}</ul>,
                        ol: ({ children }) => <ol className="readme-ol">{children}</ol>,
                        li: ({ children }) => <li className="readme-li">{children}</li>,
                        a: ({ href, children }) => (
                            <a
                                href={href}
                                className="readme-link"
                                target="_blank"
                                rel="noopener noreferrer"
                            >
                                {children}
                            </a>
                        ),
                        code: ({ inline, className, children, ...props }) => {
                            const match = /language-(\w+)/.exec(className || '');
                            const language = match ? match[1] : '';
                            return !inline && language ? (
                                <SyntaxHighlighter
                                    style={isMinecraftTheme ? darcula : vscDarkPlus}
                                    language={language}
                                    PreTag="div"
                                    className="readme-code-block"
                                    {...props}
                                >
                                    {String(children).replace(/\n$/, '')}
                                </SyntaxHighlighter>
                            ) : (
                                <code className="readme-inline-code" {...props}>
                                    {children}
                                </code>
                            );
                        },
                        blockquote: ({ children }) => (
                            <blockquote className="readme-blockquote">{children}</blockquote>
                        ),
                        table: ({ children }) => <table className="readme-table">{children}</table>,
                        thead: ({ children }) => <thead className="readme-thead">{children}</thead>,
                        tbody: ({ children }) => <tbody className="readme-tbody">{children}</tbody>,
                        tr: ({ children }) => <tr className="readme-tr">{children}</tr>,
                        th: ({ children }) => <th className="readme-th">{children}</th>,
                        td: ({ children }) => <td className="readme-td">{children}</td>,
                    }}
                >
                    {previewContent}
                </ReactMarkdown>

                {isTruncated && (
                    <div className="text-gray-500 italic mt-2">... (truncated)</div>
                )}
            </div>
        </div>
    );
};
