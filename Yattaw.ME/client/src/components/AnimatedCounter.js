import React, { useEffect, useState, useRef } from 'react';

const AnimatedCounter = ({ value, duration = 1500, triggerKey = null }) => {
  const [count, setCount] = useState(0);
  const countRef = useRef(null);

  // Reset and animate when value changes OR triggerKey changes
  useEffect(() => {
    // Clear any existing animation
    if (countRef.current) {
      clearInterval(countRef.current);
    }
    
    // Make sure we have a valid value to animate to
    const targetValue = typeof value === 'number' && !isNaN(value) ? value : 0;
    
    // Reset to zero to restart animation
    setCount(0);
    
    // Don't animate if the target is 0
    if (targetValue === 0) {
      setCount(0);
      return;
    }
    
    // Animation settings
    const steps = 30;
    const stepDuration = duration / steps;
    let currentStep = 0;
    
    // Start the animation
    countRef.current = setInterval(() => {
      currentStep++;
      const progress = currentStep / steps;
      const nextCount = Math.floor(progress * targetValue);
      setCount(nextCount);
      
      // End animation
      if (currentStep === steps) {
        setCount(targetValue);
        clearInterval(countRef.current);
      }
    }, stepDuration);
    
    // Cleanup on unmount
    return () => {
      if (countRef.current) {
        clearInterval(countRef.current);
      }
    };
  }, [value, duration, triggerKey]); // Added triggerKey as dependency
  
  return <>{count}</>;
};


export default AnimatedCounter;